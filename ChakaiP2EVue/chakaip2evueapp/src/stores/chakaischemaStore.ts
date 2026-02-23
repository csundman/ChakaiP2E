import { createSimpleStore } from './baseStore';
import type { ChakaiSchema } from '@/types';

const store = createSimpleStore<ChakaiSchema>('/chakaischema/');

function getIdsByTable(tableName: string): number[] {
    return store.items
        .filter(schema => schema.ChakaiSchemaTable === tableName)
        .map(schema => schema.ChakaiSchemaId);
}

function getIdNamePairsByTable(tableName: string): { ChakaiSchemaId: number; ChakaiSchemaName: string; ChakaiSchemaShortName: string }[] {
    return store.items
        .filter(schema => schema.ChakaiSchemaTable === tableName)
        .map(({ ChakaiSchemaId, ChakaiSchemaName, ChakaiSchemaShortName }) => ({ ChakaiSchemaId, ChakaiSchemaName, ChakaiSchemaShortName }));
}

export const chakaischemaStore = {
    ...store,
    getIdsByTable,
    getIdNamePairsByTable
};
