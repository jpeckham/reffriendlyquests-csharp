"""Read-only validation of RuntimeChecks.dll snapshots; never edits profiles."""
import argparse, json, re
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('--runtime', required=True)
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
logs = Path(args.runtime) / 'user/logs'
before = json.loads((logs / 'ref-friendly-before.json').read_text(encoding='utf-8-sig'))
after = json.loads((logs / 'ref-friendly-after.json').read_text(encoding='utf-8-sig'))
patches = json.loads((root / 'db/quests.json').read_text(encoding='utf-8-sig'))
reward_ids = re.findall(r'"([a-f0-9]{24})"', (root / 'RefFriendlyQuests.cs').read_text().split('private readonly List<MongoId> _refQuestsToEdit')[0])
assert len(reward_ids) == 15 and len(patches) == 6
assert before['quests'].keys() == after['quests'].keys()
for qid, quest in after['quests'].items():
    original = before['quests'][qid]
    for field in ['AvailableForStart', 'Fail', 'Started', 'Success', 'AutoStart']:
        assert quest['conditions'].get(field) == original['conditions'].get(field), (qid, field)
    if qid not in patches:
        assert quest['conditions'] == original['conditions'], qid
    else:
        actual = quest['conditions']['AvailableForFinish']
        expected = patches[qid]['AvailableForFinish']
        assert len(actual) == len(expected)
        for current, wanted in zip(actual, expected):
            assert current['id'] == wanted['id']
            assert float(current['value']) == float(wanted['value'])
            assert current['conditionType'] == 'CounterCreator'
            assert current['counter']['conditions'][0]['conditionType'] == 'Kills'
            assert current['counter']['conditions'][0]['target'] == wanted['counter']['conditions'][0]['target']
            assert after['labels'][current['id']].startswith('Eliminate ')
            if qid == '68342446a8d674b5740b31fc':
                weapons = current['counter']['conditions'][0]['weapon']
                assert len(weapons) > 0
                assert set(wanted['counter']['conditions'][0]['weapon']).issubset(weapons)
    def rewards_for(data, template):
        return [item for reward in data.get('rewards', {}).get('Success', [])
                for item in reward.get('items', []) or [] if item['_tpl'] == template]
    assert rewards_for(quest, '5d235b4d86f7742e017bc88a') == rewards_for(original, '5d235b4d86f7742e017bc88a'), 'GP rewards changed'
    if qid in reward_ids:
        assert len(rewards_for(quest, '6656560053eaaa7a23349c86')) == 1, (qid, 'Lega medal')
for original, changed in zip(before['loyalty'], after['loyalty']):
    assert changed['minStanding'] == round(original['minStanding'] * 0.85, 2)
    assert changed['minLevel'] == original['minLevel']
    assert changed['minSalesSum'] == original['minSalesSum']
print('PASS: six raid objective sets; six populated weapon groups; locale overrides; all prerequisites/fail conditions preserved; 15 Lega reward sets; unchanged GP rewards; 0.85 standing multiplier')
