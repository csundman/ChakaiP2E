<template>
    <div class="actions-container">

        <div class="header-row">
            <h2>Actions</h2>
            <button class="btn add" @click="handleButtonAddAction">Add Action</button>
        </div>


        <!-- Error message -->
        <div v-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>
        
        <!-- Action Table -->
        <ActionsGrid 
            :title="'All Actions'"
            :actions="actions" 
            :errorMessage="errorMessage"
            @action-clicked="handleActionClicked" 
        />


        <!-- Add Modal -->
        <ModalCreateForm 
            ref="addModal"
            title="Create New Action"
            :fields="actionFormFields" 
            :onSubmit="handleSubmitAddAction"
            :errorMessage="submitErrorMessage" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts';
import type { Action } from '@/types/';
import { actionStore } from '@/stores/';
import { chakaischemaStore } from '@/stores/';
import ActionsGrid from '@/components/ActionsGrid.vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const submitErrorMessage = ref<string | null>(null);

const actions = computed(() => actionStore.items || []);

const errorMessage = computed<null | string>(() => actionStore.errorMessage)

const handleActionClicked = (action: Action) => {
    router.push(`/actions/${action.ActionId}`); // or do something else with the ID
};

const addModal = ref();

// Form fields
const actionFormFields = computed(() => [
    {
        name: 'ActionName',
        label: 'Action Name',
        type: 'string',
        placeholder: 'Enter action name',
        required: true,
        value: ''
    },
    {
        name: 'ActionDescription',
        label: 'Action Description',
        type: 'textarea',
        placeholder: 'Enter action description',
        required: false,
        value: ''
    },
    {
        name: 'ActionCostId',
        label: 'Action Cost',
        type: 'searchable-select',
        required: false,
        options: chakaischemaStore.getIdNamePairsByTable('action_costs').map(s => ({
            label: s.ChakaiSchemaName,
            value: s.ChakaiSchemaId
        })),
        value: ''
    },
    {
        name: 'ActionCategoryId',
        label: 'Action Category',
        type: 'searchable-select',
        required: false,
        options: chakaischemaStore.getIdNamePairsByTable('action_categories').map(s => ({
            label: s.ChakaiSchemaName,
            value: s.ChakaiSchemaId
        })),
        value: ''
    },
    {
        name: 'GameplayModeId',
        label: 'Gameplay Mode',
        type: 'searchable-select',
        required: false,
        options: chakaischemaStore.getIdNamePairsByTable('gameplay_modes').map(s => ({
            label: s.ChakaiSchemaName,
            value: s.ChakaiSchemaId
        })),
        value: ''
    },
]);

// Handlers
const handleSubmitAddAction = async (formData: Record<string, any>) => {
    if (!formData.ActionName?.trim()) return;

    try {
        const rawPayload = {
            ActionName: formData.ActionName.trim(),
            ActionDescription: formData.ActionDescription,
            ActionCostId: formData.ActionCostId,
            ActionCategoryId: formData.ActionCategoryId,
            GameplayModeId: formData.GameplayModeId
        };

        const payload = Object.fromEntries(
            Object.entries(rawPayload).filter(([_, value]) => value !== '' && value != null)
        );

        await api.post('/actions/', payload);
        await actionStore.forceLoad();
        addModal.value.close();
    } catch (error) {
        console.error(getErrorMessage(error));
        submitErrorMessage.value = 'Could not add action. It may already exist.';
    }
};

const handleButtonAddAction = () => {
    submitErrorMessage.value = null;
    addModal.value.open();
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