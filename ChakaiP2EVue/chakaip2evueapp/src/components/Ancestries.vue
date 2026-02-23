<template>
    <div class="ancestries-container">
        <div class="header-row">
            <h2>Ancestries</h2>
            <button class="btn add" @click="handleButtonAddAncestry()">
                Add Ancestry
            </button>
        </div>

        <!-- Display error message if exists -->
        <div v-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>

        <SimpleGrid
            title="All Ancestries"
            :items="ancestries"
            idField="AncestryId"
            nameField="AncestryName"
            @delete-clicked="handleButtonDeleteAncestry"
            :errorMessage="errorMessage"
            :canDelete="true"
            :pageSize="10"
        />

        <!-- Modal for adding a new cclass -->
        <ModalCreateForm 
            ref="addModal"
            title="Create New Ancestry"
            :fields="ancestryFormFields" 
            :onSubmit="handleSubmitAddAncestry"
            :errorMessage="submitErrorMessage" />

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this ancestry?"
            confirmButtonText="Yes, Delete"
            :onConfirm="handleConfirmDeleteAncestry"
            :onCancel="handleCancelDeleteAncestry"
            :errorMessage="submitErrorMessage" />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts'
import type { Ancestry } from '@/types/'
import { ancestryStore } from '@/stores/';

const addModal = ref();
const deleteModal = ref();
const ancestryToDelete = ref<Ancestry | null>(null); // Track ancestry being deleted
const submitErrorMessage = ref<string | null>(null);

const ancestries = computed(() => ancestryStore.items);
const errorMessage = computed(() => ancestryStore.errorMessage);

// --- FORM DEFINITION ---
const ancestryFormFields = ref([
    {
        name: 'AncestryName',
        label: 'Ancestry Name',
        type: 'string',
        placeholder: 'Enter ancestry name',
        required: true,
        value: ''
    }
]);

onMounted(() => {

});

// --- HANDLERS ---
const handleSubmitAddAncestry = async (formData: Record<string, any>) => {
    if (!formData.AncestryName?.trim()) return;

    try {
        const response = await api.post('/ancestries/', {
            AncestryName: formData.AncestryName.trim()
        });

        await ancestryStore.forceLoad();
        addModal.value.close();

        // Reset form fields
        ancestryFormFields.value.forEach(f => f.value = '');
    } catch (error) {
        console.error(errorMessage.value);
        submitErrorMessage.value = 'Could not add ancestry. It may already exist.';
    }
};

const handleButtonAddAncestry = () => {
    submitErrorMessage.value = null;
    addModal.value.open();
}

// Show confirm delete modal
const handleButtonDeleteAncestry = (ancestry: Ancestry) => {
    ancestryToDelete.value = ancestry;
    deleteModal.value.open();
};

// Proceed with delete
const handleConfirmDeleteAncestry = async () => {
    if (!ancestryToDelete.value) return;

    try {
        await api.delete(`/ancestries/${ancestryToDelete.value.AncestryId}`);
        ancestryStore.forceLoad()
        ancestryToDelete.value = null;
    } catch (error) {
        console.error(errorMessage.value);
    }
};

const handleCancelDeleteAncestry = () => {
    ancestryToDelete.value = null;
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
