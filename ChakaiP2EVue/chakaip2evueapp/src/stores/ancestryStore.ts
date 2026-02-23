import { createSimpleStore } from './baseStore';
import type { Ancestry } from '@/types';

export const ancestryStore = createSimpleStore<Ancestry>('/ancestries/');