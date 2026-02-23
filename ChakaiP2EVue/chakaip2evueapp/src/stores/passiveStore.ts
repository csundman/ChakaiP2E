import { createSimpleStore } from './baseStore';
import type { Passive } from '@/types';

export const passiveStore = createSimpleStore<Passive>('/passives/');