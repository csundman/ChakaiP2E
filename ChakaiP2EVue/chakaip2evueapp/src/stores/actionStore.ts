import { createSimpleStore } from './baseStore';
import type { Action } from '@/types';

export const actionStore = createSimpleStore<Action>('/actions/');