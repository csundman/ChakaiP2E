import { createSimpleStore } from './baseStore';
import type { Background } from '@/types';

export const backgroundStore = createSimpleStore<Background>('/backgrounds/');