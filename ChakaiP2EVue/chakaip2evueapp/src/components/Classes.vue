<template>
    <div class="cclasses-container">
        <div class="header-row">
            <h2>Classes</h2>
            <button class="btn add" @click="handleButtonAddCClass()">
                Add Class
            </button>
        </div>

        <!-- Display error message if exists -->
        <div v-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>

        <SimpleGrid
            title="All Classes"
            :items="cclasses"
            idField="CClassId"
            nameField="ClassName"
            @delete-clicked="handleButtonDeleteCClass"
            :errorMessage="errorMessage"
            :canDelete="true"
            :pageSize="10"
        />

        <!-- Modal for adding a new cclass -->
        <ModalCreateForm 
            ref="addModal"
            title="Create New Class"
            :fields="cclassFormFields" 
            :onSubmit="handleSubmitAddCClass"
            :errorMessage="submitErrorMessage" />

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this class?"
            confirmButtonText="Yes, Delete"
            :onConfirm="handleConfirmDeleteCClass"
            :onCancel="handleCancelDeleteCClass"
            :errorMessage="submitErrorMessage"
        />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts'
import type { CClass } from '@/types/'
import { cclassStore } from '@/stores/';

const addModal = ref();
const deleteModal = ref();
const cclassToDelete = ref<CClass | null>(null); // Track cclass being deleted
const submitErrorMessage = ref<string | null>(null);

const cclasses = computed(() => cclassStore.items);
const errorMessage = computed(() => cclassStore.errorMessage);

// --- FORM DEFINITION ---
const cclassFormFields = ref([
    {
        name: 'ClassName',
        label: 'Class Name',
        type: 'string',
        placeholder: 'Enter class name',
        required: true,
        value: ''
    }
]);

onMounted(() => {

});

// --- HANDLERS ---
const handleSubmitAddCClass = async (formData: Record<string, any>) => {
    if (!formData.ClassName?.trim()) return;

    try {
        const response = await api.post('/classes/', {
            ClassName: formData.ClassName.trim()
        });

        await cclassStore.forceLoad();
        addModal.value.close();
    } catch (error) {
        console.error(errorMessage.value);
        submitErrorMessage.value = 'Could not add class. It may already exist.';
    }
};

const handleButtonAddCClass = () => {
    submitErrorMessage.value = null;
    addModal.value.open();
}

// Show confirm delete modal
const handleButtonDeleteCClass = (cclass: CClass) => {
    cclassToDelete.value = cclass;
    deleteModal.value.open();
};

// Proceed with delete
const handleConfirmDeleteCClass = async () => {
    if (!cclassToDelete.value) return;

    try {
        await api.delete(`/classes/${cclassToDelete.value.CClassId}`);
        cclassStore.forceLoad()
        cclassToDelete.value = null;
    } catch (error) {
        console.error(errorMessage.value);
    }
};

const handleCancelDeleteCClass = () => {
    cclassToDelete.value = null;
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
