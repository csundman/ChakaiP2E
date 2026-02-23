import type { Trait } from './trait';
import type { Action } from './action';

export interface Passive {
    PassiveId: number;
    PassiveName: string;
    PassiveDescription: string;
    PassiveTypeId: number;
    PassiveTypeName: string;

    Traits: Trait[];
    Rolls: { RollId: number }[];
    Actions: Action[];
}
