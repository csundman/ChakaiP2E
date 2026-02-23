import { createSimpleStore } from './baseStore';
import type { CClass } from '@/types';

export const cclassStore = createSimpleStore<CClass>('/classes/');