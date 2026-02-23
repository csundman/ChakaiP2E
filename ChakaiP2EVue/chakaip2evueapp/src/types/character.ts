import type { Action } from './action';
import type { Passive } from './passive';

export interface Character {
    CharacterId: number;
    CharacterName: string;
    AncestryId: number;
    AncestryName: string;
    BackgroundId: number;
    BackgroundName: string;
    CClassId: number;
    ClassName: string;

    Actions: Action[];
    Passives: Passive[];
}