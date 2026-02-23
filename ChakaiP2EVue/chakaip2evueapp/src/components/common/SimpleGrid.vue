<template>
    <!-- Error message -->
    <div v-if="props.errorMessage" class="error-message">
        {{ props.errorMessage }}
    </div>

    <!-- Toolbar: Title + Search Bar -->
    <div class="grid-toolbar">
        <div class="grid-title" v-if="props.title">
            {{ props.title }}
        </div>

        <input
            type="text"
            v-model="searchQuery"
            placeholder="Search items..."
            class="search-bar"
        />
    </div>

    <!-- Item Table -->
    <div class="table-wrapper">
        <table class="item-table">
            <thead>
                <tr>
                    <th @click="setSort('name')" style="cursor: pointer">
                        Name
                        <span v-if="sortKey === 'name'">{{ sortAsc ? '▲' : '▼' }}</span>
                    </th>
                    <th 
                        @click="props.descriptionField ? setSort('description') : null"
                        :style="{ cursor: props.descriptionField ? 'pointer' : 'default' }"
                    >
                        <span v-if="descriptionField">Description</span>
                        <span v-if="sortKey === 'description'">{{ sortAsc ? '▲' : '▼' }}</span>
                    </th>
                    <th v-if="canDelete"></th>
                </tr>
            </thead>

            <tbody>
                <tr
                    v-for="item in pagedItems"
                    :key="item[props.idField]"
                    class="clickable-row"
                    @dblclick="itemClicked(item)"
                >
                    <td>{{ item[props.nameField] }}</td>
                    <td>{{ item[props.descriptionField] }}</td>
                    <td v-if="canDelete"><button class="btn delete" @click="deleteButtonClicked(item)" >Delete</button></td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="pagination-controls">
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
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';

const emit = defineEmits<{ 
    (e: 'item-clicked', id: any): void; 
    (e: 'delete-clicked', id: any): void;
}>();

const props = defineProps<{
    title: string | null;
    items: Array<Record<string, any>>;
    idField: string;
    nameField: string;
    descriptionField: string;
    errorMessage: string | null;
    canDelete?: boolean;
    pageSize?: number;
}>();

const pageNum = ref(1);
const pageSize = ref(props.pageSize ?? 10);

const searchQuery = ref('');
const sortKey = ref<'name' | 'description'>('name');
const sortAsc = ref(true);

function setSort(key: typeof sortKey.value) {
    if (sortKey.value === key) {
        sortAsc.value = !sortAsc.value;
    } else {
        sortKey.value = key;
        sortAsc.value = true;
    }
}

const filteredItems = computed(() => {
    const query = searchQuery.value.trim().toLowerCase();

    const nameField = props.nameField;
    const descField = props.descriptionField;

    return [...props.items]
        .filter(item => {
            if (!query) return true;

            const name = String(item[nameField] ?? '').toLowerCase();
            const desc = String(item[descField] ?? '').toLowerCase();

            return name.includes(query) || desc.includes(query);
        })
        .sort((a, b) => {
            const field =
                sortKey.value === 'name' ? props.nameField : props.descriptionField;

            const aVal = String(a[field] ?? '').toLowerCase();
            const bVal = String(b[field] ?? '').toLowerCase();

            if (aVal < bVal) return sortAsc.value ? -1 : 1;
            if (aVal > bVal) return sortAsc.value ? 1 : -1;
            return 0;
        });
});

const pagedItems = computed(() => {
    const start = (pageNum.value - 1) * pageSize.value;
    return filteredItems.value.slice(start, start + pageSize.value);
});


const totalPages = computed(() => {
    return Math.max(1, Math.ceil(filteredItems.value.length / pageSize.value));
});

function itemClicked(item: any) {
    emit('item-clicked', item);
}

function deleteButtonClicked(item: any) {
    emit('delete-clicked', item);
}
</script>

<style scoped>
/* unchanged styling */
.table-wrapper {
    margin-bottom: 16px;
}
.item-table {
    width: 100%;
    border-collapse: collapse;
    background-color: white;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
}
.item-table th,
.item-table td {
    padding: 12px;
    text-align: left;
    border-bottom: 1px solid #ddd;
    font-size: 0.95rem;
}
.item-table th {
    background-color: #f5f5f5;
    font-weight: 600;
}
.item-table th:nth-child(1),
.item-table td:nth-child(1) {
    width: 20%;
}
.item-table th:nth-child(2),
.item-table td:nth-child(2) {
    width: 80%;
}
.clickable-row {
    transition: background-color 0.2s, transform 0.1s ease-out;
    cursor: pointer;
}
.clickable-row:active {
    background-color: #ddd;
    transform: scale(0.98);
}
.clickable-row:hover {
    background-color: #f5f5f5;
    transform: scale(1.02);
}

@media (prefers-color-scheme: dark) {
    .item-table {
        background-color: #1e1e1e;
        box-shadow: 0 2px 8px rgba(255, 255, 255, 0.05);
    }
    .item-table th {
        background-color: #2a2a2a;
        color: #ddd;
        border-bottom: 1px solid #444;
    }
    .item-table td {
        color: #e0e0e0;
        border-bottom: 1px solid #333;
    }
    .clickable-row {
        background-color: #2a2a2a;
        color: #e0e0e0;
    }
    .clickable-row:active {
        background-color: #444;
    }
    .clickable-row:hover {
        background-color: #333;
    }
}
</style>
