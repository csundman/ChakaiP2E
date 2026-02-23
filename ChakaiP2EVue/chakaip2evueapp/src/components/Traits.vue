<template>
    <div class="traits-container">
        <div class="header-row">
            <h2>Traits</h2>
            <button class="btn add" @click="handleButtonAddTrait()">
                Add Trait
            </button>
        </div>

        <!-- Display error message if exists -->
        <div v-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>

        <SimpleGrid
            title="All Traits"
            :items="traits"
            idField="TraitId"
            nameField="TraitName"
            descriptionField="TraitDescription"
            @item-clicked="handleClickedTrait"
            @delete-clicked="handleButtonDeleteTrait"
            :errorMessage="errorMessage"
            :canDelete="true"
            :pageSize="10"
        />

        <!-- Modal for adding a new trait -->
        <ModalCreateForm 
            ref="addModal"
            title="Create New Trait"
            :fields="traitFormFields" 
            :onSubmit="handleSubmitAddTrait"
            :errorMessage="submitErrorMessage" />

                <!-- Modal for adding a new trait -->
        <ModalCreateForm 
            ref="editModal"
            :title="`Edit Trait - ${traitToEdit?.TraitName ?? ''}`"
            :fields="traitEditFormFields" 
            :onSubmit="handleSubmitEditTrait"
            :onCancel="handleCancelEditTrait"
            :errorMessage="submitErrorMessage" 
            :defaultValues="traitToEdit"/>

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this trait?"
            confirmButtonText="Yes, Delete"
            :onConfirm="handleConfirmDeleteTrait"
            :onCancel="handleCancelDeleteTrait"
            :errorMessage="submitErrorMessage"
        />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts'
import type { Trait } from '@/types/trait'
import { traitStore } from '@/stores/traitStore';

const traitToDelete = ref<Trait | null>(null);
const submitErrorMessage = ref<string | null>(null);
const traitToEdit = ref<Trait | null>(null);
const addModal = ref();
const editModal = ref();
const deleteModal = ref();

const traits = computed(() => traitStore.items);
const errorMessage = computed(() => traitStore.errorMessage);

// --- FORM DEFINITION ---
const traitFormFields = ref([
    {
        name: 'TraitName',
        label: 'Trait Name',
        type: 'string',
        placeholder: 'Enter trait name',
        required: true,
        value: ''
    },
    {
        name: 'TraitDescription',
        label: 'Trait Description',
        type: 'string',
        placeholder: 'Enter trait description',
        required: false,
        value: ''
    }
]);

// --- FORM DEFINITION ---
const traitEditFormFields = ref([
    {
        name: 'TraitDescription',
        label: 'Trait Description',
        type: 'string',
        placeholder: 'Enter trait description',
        required: false,
        value: ''
    }
]);

onMounted(() => {
    // Optionally load traits if needed
});

// --- HANDLERS ---
const handleSubmitAddTrait = async (formData: Record<string, any>) => {
    if (!formData.TraitName?.trim()) return;

    try {
        await api.post('/traits/', {
            TraitName: formData.TraitName.trim(),
            TraitDescription: formData.TraitDescription.trim()
        });
        await traitStore.forceLoad();
        addModal.value.close()
    } catch (error) {
        console.error(errorMessage.value);
        submitErrorMessage.value = 'Could not add trait. It may already exist.';
    }
};

const handleSubmitEditTrait = async (formData: Record<string, any>) => {
    if (!traitToEdit.value) return;

    try {
        await api.put(`/traits/${traitToEdit.value.TraitId}`, {
            TraitDescription: formData.TraitDescription.trim()
        });
        await traitStore.forceLoad();
        editModal.value.close()
        traitEditFormFields.value.forEach(f => f.value = '');
        traitToEdit.value = null;
    } catch (error) {
        console.error(errorMessage.value);
        submitErrorMessage.value = 'Could not edit trait.';
    }
};

const handleCancelEditTrait = () => {
    traitToEdit.value = null;
};

const handleButtonAddTrait = () => {
    submitErrorMessage.value = null;
    addModal.value.open();
}

const handleClickedTrait = (trait: Trait) => {
    traitToEdit.value = trait;
    editModal.value.open(
        {
            TraitDescription: trait.TraitDescription
        }
    );
};

const handleButtonDeleteTrait = (trait: Trait) => {
    traitToDelete.value = trait;
    deleteModal.value.open();
};

const handleConfirmDeleteTrait = async () => {
    if (!traitToDelete.value) return;
    try {
        await api.delete(`/traits/${traitToDelete.value.TraitId}`);
        await traitStore.forceLoad();
        traitToDelete.value = null;
    } catch (error) {
        console.error(errorMessage.value);
    }
};

const handleCancelDeleteTrait = () => {
    traitToDelete.value = null;
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
