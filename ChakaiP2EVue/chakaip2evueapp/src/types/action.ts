import type { Trait } from './trait';

export interface Action {
    ActionId: number;
    ActionName: string;
    ActionDescription: string;
    ActionCostId: number;
    ActionCostName: string;
    ActionCategoryId: number;
    ActionCategoryName: string;
    GameplayModeId: number;
    GameplayModeName: string;
    SystemDefined: boolean;

    Traits: Trait[];
    Rolls: { RollId: number }[];
}
