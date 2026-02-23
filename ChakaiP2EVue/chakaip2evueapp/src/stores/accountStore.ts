import { createSimpleStore } from './baseStore';
import type { Account } from '@/types';

export const accountStore = createSimpleStore<Account>('/accounts/');