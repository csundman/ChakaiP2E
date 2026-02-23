<template>
    <!-- Error message -->
    <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
    </div>
    
    <!-- Action Table -->
    <div class="table-wrapper">
        <!-- Toolbar: Title + Search Bar -->
        <div class="grid-toolbar">
            <div class="grid-title" v-if="props.title">
                {{ props.title }}
            </div>

            <input
                type="text"
                v-model="searchQuery"
                placeholder="Search by name..."
                class="search-bar"
            />

            <button
                :class="['btn filter', showFilters ? 'clear-mode' : 'show-mode']"
                @click="toggleFilters"
            >
                {{ 
                    // showFilters ? 'Clear Filters' : 'Show Filters' 
                    '🧹'
                }}
            </button>
        </div>
        <div class="filter-controls">
            

            <!-- Filter Controls (animated unfold) -->
            <transition name="unfold">
                <div v-show="showFilters" class="filter-area">
                    <!-- Dropdown Filters -->
                    <div class="dropdown-filters">
                        <select v-model="selectedCostId">
                            <option value="">All Costs</option>
                            <option
                                v-for="cost in chakaischemaStore.getIdNamePairsByTable('action_costs')"
                                :key="cost.ChakaiSchemaId"
                                :value="cost.ChakaiSchemaId"
                            >
                                {{ isMobile ? (cost.ChakaiSchemaName || cost.ChakaiSchemaName) : cost.ChakaiSchemaName }}
                            </option>
                        </select>

                        <select v-model="selectedCategoryId">
                            <option value="">All Categories</option>
                            <option
                                v-for="category in chakaischemaStore.getIdNamePairsByTable('action_categories')"
                                :key="category.ChakaiSchemaId"
                                :value="category.ChakaiSchemaId"
                            >
                                {{ isMobile ? (category.ChakaiSchemaName || category.ChakaiSchemaName) : category.ChakaiSchemaName }}
                            </option>
                        </select>

                        <select v-model="selectedModeId">
                            <option value="">All Modes</option>
                            <option
                                v-for="mode in chakaischemaStore.getIdNamePairsByTable('gameplay_modes')"
                                :key="mode.ChakaiSchemaId"
                                :value="mode.ChakaiSchemaId"
                            >
                                {{ isMobile ? (mode.ChakaiSchemaName || mode.ChakaiSchemaName) : mode.ChakaiSchemaName }}
                            </option>
                        </select>
                    </div>
                </div>
            </transition>
        </div>

        <table class="action-table">
            <thead>
                <tr>
                    <th @click="setSort('name')" style="cursor: pointer">
                        Name
                        <span v-if="sortKey === 'name'">{{ sortAsc ? '▲' : '▼' }}</span>
                    </th>
                    <th @click="setSort('cost')" style="cursor: pointer">
                        Cost
                        <span v-if="sortKey === 'cost'">{{ sortAsc ? '▲' : '▼' }}</span>
                    </th>
                    <th @click="setSort('category')" style="cursor: pointer">
                        Category
                        <span v-if="sortKey === 'category'">{{ sortAsc ? '▲' : '▼' }}</span>
                    </th>
                    <th @click="setSort('mode')" style="cursor: pointer">
                        Mode
                        <span v-if="sortKey === 'mode'">{{ sortAsc ? '▲' : '▼' }}</span>
                    </th>

                </tr>
            </thead>
            <tbody>
                <tr v-for="action in pagedActions" :key="action.ActionId" 
                    class="clickable-row"
                    @dblclick="goToActionDetails(action)"
                >
                    <td>{{ action.ActionName }}</td>
                    <td>{{ getCostDisplay(action.ActionCostId) }}</td>
                    <td>{{ getNameById('action_categories', action.ActionCategoryId, isMobile) }}</td>
                    <td>{{ getNameById('gameplay_modes', action.GameplayModeId, isMobile) }}</td>

                </tr>
            </tbody>
        </table>

        <div class="pagination-controls" style="margin-bottom: 20px;">
            <button 
                class="page-btn" 
                :disabled="pageNum === 1"
                @click="pageNum--"
            >
                ⬅️ Prev
            </button>

            <span class="page-info">
                Page {{ pageNum }} / {{ totalPages }}
            </span>

            <button 
                class="page-btn" 
                :disabled="pageNum === totalPages"
                @click="pageNum++"
            >
                Next ➡️
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import type { Action } from '@/types/';
import { chakaischemaStore } from '@/stores/';
import { COST_ALIASES } from '@/utils/costAliases';

const props = defineProps<{
    title: string,
    actions: Action[],
    errorMessage: string | null;
    pageSize?: number;
}>();

// Modal visibility
const showFilters = ref(false)

// Filter state
const selectedCostId = ref('');
const selectedCategoryId = ref('');
const selectedModeId = ref('');
const isMobile = ref(window.innerWidth < 768);
const searchQuery = ref('');

const pageNum = ref(1);
const pageSize = ref(props.pageSize ?? 10);

const updateIsMobile = () => {
    isMobile.value = window.innerWidth < 768;
};

onMounted(() => {
    window.addEventListener('resize', updateIsMobile);
});

onBeforeUnmount(() => {
    window.removeEventListener('resize', updateIsMobile);
});

const emit = defineEmits<{
    (e: 'action-clicked', action: Action): void
}>();

const goToActionDetails = (action: Action) => {
    console.info("Action-clicked");
    emit('action-clicked', action);
};

function toggleFilters() {
    if (showFilters.value) {
        // If visible → clear all and hide
        clearFilters();
        searchQuery.value = '';
        showFilters.value = false;
    } else {
        // If hidden → show
        showFilters.value = true;
    }
}

function clearFilters() {
    selectedCostId.value = '';
    selectedCategoryId.value = '';
    selectedModeId.value = '';
}

const filteredActions = computed(() => {
    const query = searchQuery.value.trim().toLowerCase();

    return [...props.actions]
        .filter(action =>
            (!selectedCostId.value || action.ActionCostId == Number(selectedCostId.value)) &&
            (!selectedCategoryId.value || action.ActionCategoryId == Number(selectedCategoryId.value)) &&
            (!selectedModeId.value || action.GameplayModeId == Number(selectedModeId.value)) &&
            (!query || action.ActionName?.toLowerCase().includes(query))
        )
        .sort((a, b) => {
            let aVal: string | number;
            let bVal: string | number;

            switch (sortKey.value) {
                case 'name':
                    aVal = a.ActionName.toLowerCase();
                    bVal = b.ActionName.toLowerCase();
                    break;
                case 'cost':
                    aVal = a.ActionCostId;
                    bVal = b.ActionCostId;
                    break;
                case 'category':
                    aVal = a.ActionCategoryId;
                    bVal = b.ActionCategoryId;
                    break;
                case 'mode':
                    aVal = a.GameplayModeId;
                    bVal = b.GameplayModeId;
                    break;
            }

            if (aVal < bVal) return sortAsc.value ? -1 : 1;
            if (aVal > bVal) return sortAsc.value ? 1 : -1;
            return 0;
        });
});

const pagedActions = computed(() => {
    const start = (pageNum.value - 1) * pageSize.value;
    return filteredActions.value.slice(start, start + pageSize.value);
});


const totalPages = computed(() => {
    return Math.max(1, Math.ceil(filteredActions.value.length / pageSize.value));
});


// Helpers
const getNameById = (table: string, id: string | number | null | undefined, useShort = false): string => {
    if (!id) return '';
    const match = chakaischemaStore.getIdNamePairsByTable(table).find(entry => entry.ChakaiSchemaId === id);
    if (!match) return '';
    return useShort ? match.ChakaiSchemaShortName || match.ChakaiSchemaName : match.ChakaiSchemaName;
};

const getCostDisplay = (costId: string | number | null | undefined): string => {
    const raw = getNameById('action_costs', costId, isMobile.value);
    if (!raw) 
        return '';
    return COST_ALIASES[raw] ?? raw;
};

const sortKey = ref<'name' | 'cost' | 'category' | 'mode'>('name');
const sortAsc = ref(true);

function setSort(key: typeof sortKey.value) {
    if (sortKey.value === key) {
        sortAsc.value = !sortAsc.value;
    } else {
        sortKey.value = key;
        sortAsc.value = true;
    }
}


</script>

<style scoped>

.table-wrapper {
    margin-bottom: 16px;
}

.filter-controls {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-bottom: 5px;
    margin-top: 16px;
    width: 100%;
}

.filter-title {
    font-size: 1.15rem;
    font-weight: 700;
}

.filter-area {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

/* Make the filter area clip overflow and animate via scaleY for symmetric timing */
.filter-area {
    overflow: hidden;
    transform-origin: top;
}

.unfold-enter-active, .unfold-leave-active {
    transition: transform 0.32s cubic-bezier(.2,.9,.2,1), opacity 0.22s ease;
}
.unfold-enter-from, .unfold-leave-to {
    transform: scaleY(0);
    opacity: 0;
}
.unfold-enter-to, .unfold-leave-from {
    transform: scaleY(1);
    opacity: 1;
}

.dropdown-filters {
    display: flex;
    gap: 10px; /* Optional: adjust spacing between dropdowns */
    width: 100%;
}

.dropdown-filters select {
    flex: 1;              /* Equal width for all */
    padding: 8px 10px;
    border: 1px solid #ccc;
    border-radius: 6px;
    font-size: 0.95rem;
    min-width: 0;         /* Important: allows shrinking properly */
}

.filter-toggle-btn.show-mode {
    background-color: #3498db; /* blue */
    color: white;
}

.filter-toggle-btn.clear-mode {
    background-color: #e74c3c; /* red */
    color: white;
}

.action-table {
    width: 100%;
    border-collapse: collapse;
    background-color: white;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
}

.action-table th,
.action-table td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid #ddd;
    font-size: 0.95rem;
}

.action-table th {
    background-color: #e0dede;
    font-weight: 600;
}
.action-table th:nth-child(1),
.action-table td:nth-child(1) {
    width: 60%;
}

.action-table th:nth-child(2),
.action-table td:nth-child(2) {
    width: 10%;
}
.action-table th:nth-child(3),
.action-table td:nth-child(3) {
    width: 20%;
}

.action-table th:nth-child(4),
.action-table td:nth-child(4) {
    width: 10%;
}

.clickable-row {
    transition: background-color 0.2s, transform 0.1s ease-out; /* Smooth transition */
    cursor: pointer;
}

.clickable-row:active {
    background-color: #ddd;  /* Light gray background when pressed */
    transform: scale(0.98);  /* Slightly shrink for pressed effect */
}

/* Optional: Add hover effect for better UX */
.clickable-row:hover {
    background-color: #f5f5f5; /* Light hover color */
    transform: scale(1.02);
}

@media (prefers-color-scheme: dark) {
    .action-table {
        background-color: #1e1e1e;
        box-shadow: 0 2px 8px rgba(255, 255, 255, 0.05);
    }

    .action-table th {
        background-color: #2a2a2a;
        color: #ddd;
        border-bottom: 1px solid #444;
    }

    .action-table td {
        color: #e0e0e0;
        border-bottom: 1px solid #333;
    }

    .search-bar {
        background-color: #444;  /* Dark background for search bar */
        border: 1px solid #555;  /* Darker border for search input */
        color: #e0e0e0;  /* Light text color for search bar */
    }

    .dropdown-filters select {
        background-color: #444;  /* Dark background for dropdowns */
        border: 1px solid #555;  /* Dark border for dropdowns */
        color: #e0e0e0;  /* Light text color for dropdown options */
    }

    .clickable-row {
        background-color: #2a2a2a;  /* Dark background for rows */
        color: #e0e0e0;  /* Light text color */
    }

    .clickable-row:active {
        background-color: #444;  /* Darker background when pressed */
    }

    .clickable-row:hover {
        background-color: #333; /* Slightly lighter background on hover */
    }
}

</style>