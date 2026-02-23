import { reactive } from 'vue';
import api from '@/api';
import { getErrorMessage } from '@/utils/error';

export function createSimpleStore<T>(endpoint: string) {
    const state = reactive({
        items: [] as T[],
        errorMessage: null as string | null,
        loading: false,
        loaded: false,
        attempted: false
    });

    async function forceLoad() {
        state.loading = true;
        state.attempted = true;
        try {
            const res = await api.get(endpoint);
            state.items = res.data;
            state.errorMessage = '';
            state.loaded = true;
        } catch (error) {
            setError(getErrorMessage(error));
        } finally {
            state.loading = false;
        }
    }

    async function load() {
        if (state.loading || state.attempted) return;
        await forceLoad();
    }

    function setError(message: string | null) {
        state.errorMessage = message;
    }

    function reset() {
        state.items.length = 0;
        state.errorMessage = null; // Clear the error message
        state.loading = false; // Reset loading state
        state.loaded = false; // Reset loaded state
        state.attempted = false; // Reset attempted state
    }

    return {
        get items() {
            if ((!state.attempted && !state.loading) || state.items.length === 0) {
                load();
            }
            return state.items;
        },
        get loading() {
            return state.loading;
        },
        get isLoaded() {
            return state.loaded && !state.loading;
        },
        get errorMessage() {
            return state.errorMessage;
        },
        load,
        forceLoad,
        setError,
        reset // Expose the reset function
    };
}
