import { createSimpleStore } from './baseStore';
import type { Character } from '@/types';

export const characterStore = createSimpleStore<Character>('/characters/');