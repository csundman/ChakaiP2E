import { createSimpleStore } from './baseStore';
import type { Trait } from '@/types';

export const traitStore = createSimpleStore<Trait>('/traits/');
