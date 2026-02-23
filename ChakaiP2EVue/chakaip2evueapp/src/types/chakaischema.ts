export interface ChakaiSchema {
    ChakaiSchemaId: number;
    ChakaiSchemaName: string;
    ChakaiSchemaShortName: string;
    ChakaiSchemaTable: string;
}

export const COST_ALIASES: Record<string, string> = {
    '1 Action': '🔷',
    '2 Action': '🔷🔷',
    '3 Action': '🔷🔷🔷',
    'Reaction': '↺',
    'Free Action': '◇',
};
