<template>
    <div class="backgrounds-container">
        <div class="header-row">
            <h2>Backgrounds</h2>
            <button class="btn add" @click="handleButtonAddBackground()">
                Add Background
            </button>
        </div>

        <!-- Display error message if exists -->
        <div v-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>

        <SimpleGrid
            title="All Backgrounds"
            :items="backgrounds"
            idField="BackgroundId"
            nameField="BackgroundName"
            @delete-clicked="handleButtonDeleteBackground"
            :errorMessage="errorMessage"
            :canDelete="true"
            :pageSize="10"
        />

        <!-- Modal for adding a new cclass -->
        <ModalCreateForm 
            ref="addModal"
            title="Create New Background"
            :fields="backgroundFormFields" 
            :onSubmit="handleSubmitAddBackground"
            :errorMessage="submitErrorMessage" />

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this background?"
            confirmButtonText="Yes, Delete"
            :onConfirm="handleConfirmDeleteBackground"
            :onCancel="handleCancelDeleteBackground"
            :errorMessage="submitErrorMessage" />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts'
import type { Background } from '@/types/'
import { backgroundStore } from '@/stores/';

const addModal = ref();
const deleteModal = ref();
const backgroundToDelete = ref<Background | null>(null); // Track background being deleted
const submitErrorMessage = ref<string | null>(null);

const backgrounds = computed(() => backgroundStore.items);
const errorMessage = computed(() => backgroundStore.errorMessage);

// --- FORM DEFINITION ---
const backgroundFormFields = ref([
    {
        name: 'BackgroundName',
        label: 'Background Name',
        type: 'string',
        placeholder: 'Enter background name',
        required: true,
        value: ''
    }
]);

onMounted(() => {

});

// --- HANDLERS ---
const handleSubmitAddBackground = async (formData: Record<string, any>) => {
    if (!formData.BackgroundName?.trim()) return;

    try {
        const response = await api.post('/backgrounds/', {
            BackgroundName: formData.BackgroundName.trim()
        });

        await backgroundStore.forceLoad();
        addModal.value.close();

        // Reset form fields
        backgroundFormFields.value.forEach(f => f.value = '');
    } catch (error) {
        console.error(errorMessage.value);
        submitErrorMessage.value = 'Could not add background. It may already exist.';
    }
};

const handleButtonAddBackground = () => {
    submitErrorMessage.value = null;
    addModal.value.open();
}

// Show confirm delete modal
const handleButtonDeleteBackground = (background: Background) => {
    backgroundToDelete.value = background;
    deleteModal.value.open();
};

// Proceed with delete
const handleConfirmDeleteBackground = async () => {
    if (!backgroundToDelete.value) return;

    try {
        await api.delete(`/backgrounds/${backgroundToDelete.value.BackgroundId}`);
        backgroundStore.forceLoad()
        backgroundToDelete.value = null;
    } catch (error) {
        console.error(errorMessage.value);
    }
};

const handleCancelDeleteBackground = () => {
    backgroundToDelete.value = null;
};

</script>

<style scoped>

.header-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    width: 100%; /* Ensure full width for proper spacing */
}

</style>
