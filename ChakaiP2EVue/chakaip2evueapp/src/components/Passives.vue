<template>
    <div class="passives-container">
        <div class="header-row">
            <h2>Passives</h2>
            <button class="btn add" @click="handleButtonAddPassive()">
                Add Passive
            </button>
        </div>

        <!-- Display error message if exists -->
        <div v-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>

        <!-- Passive Grid -->
        <PassivesGrid
            :title="''"
            :passives="passives"
            :errorMessage="errorMessage"
            @passive-clicked="handlePassiveClicked"
            @delete-passive="handleButtonDeletePassive"
        />

        <!-- Modal for adding a new passive -->
        <ModalCreateForm 
            ref="addModal"
            title="Create New Passive"
            :fields="passiveFormFields" 
            :onSubmit="handleSubmitAddPassive"
            :errorMessage="submitErrorMessage" />

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this passive?"
            confirmButtonText="Yes, Delete"
            :onConfirm="handleConfirmDeletePassive"
            :onCancel="handleCancelDeletePassive"
        />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts'
import type { Passive } from '@/types/passive'
import { passiveStore, chakaischemaStore } from '@/stores/';
import PassivesGrid from '@/components/PassivesGrid.vue';
import { useRouter } from 'vue-router';

const addModal = ref();
const deleteModal = ref();
const passiveToDelete = ref<Passive | null>(null);
const submitErrorMessage = ref<string | null>(null);

const passives = computed(() => passiveStore.items);
const errorMessage = computed(() => passiveStore.errorMessage);

const router = useRouter();

const handlePassiveClicked = (passive: Passive) => {
    router.push(`/passives/${passive.PassiveId}`);
};

// --- FORM DEFINITION ---
const passiveFormFields = ref([
    {
        name: 'PassiveName',
        label: 'Passive Name',
        type: 'string',
        placeholder: 'Enter passive name',
        required: true,
        value: ''
    },
    {
        name: 'PassiveDescription',
        label: 'Passive Description',
        type: 'textarea',
        placeholder: 'Enter passive description',
        required: false,
        value: ''
    },
    {
        name: 'PassiveTypeId',
        label: 'Passive Type',
        type: 'searchable-select',
        required: false,
        options: chakaischemaStore.getIdNamePairsByTable('passive_types').map(s => ({
            label: s.ChakaiSchemaName,
            value: s.ChakaiSchemaId
        })),
        value: ''
    },
]);

onMounted(() => {
    // Optionally load passives if needed
});

// --- HANDLERS ---
const handleSubmitAddPassive = async (formData: Record<string, any>) => {
    if (!formData.PassiveName?.trim()) return;

    try {
        await api.post('/passives/', {
            PassiveName: formData.PassiveName.trim(),
            PassiveDescription: formData.PassiveDescription.trim(),
            PassiveTypeId: formData.PassiveTypeId || null
        });
        await passiveStore.forceLoad();
        addModal.value.close();
        passiveFormFields.value.forEach(f => f.value = '');
    } catch (error) {
        console.error(errorMessage.value);
        submitErrorMessage.value = 'Could not add passive. It may already exist.';
    }
};

const handleButtonAddPassive = () => {
    submitErrorMessage.value = null;
    addModal.value.open();
}

const handleButtonDeletePassive = (passive: Passive) => {
    passiveToDelete.value = passive;
    deleteModal.value.open();
};

const handleConfirmDeletePassive = async () => {
    if (!passiveToDelete.value) return;
    try {
        await api.delete(`/passives/${passiveToDelete.value.PassiveId}`);
        await passiveStore.forceLoad();
        passiveToDelete.value = null;
    } catch (error) {
        console.error(errorMessage.value);
    }
};

const handleCancelDeletePassive = () => {
    passiveToDelete.value = null;
};

</script>

<style scoped>
.header-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    width: 100%;
}

</style>
