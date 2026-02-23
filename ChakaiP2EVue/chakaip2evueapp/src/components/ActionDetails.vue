<template>
    <div class="panel">
        <div class="top-bar">
            <template v-if="editMode">
                <button @click="cancelEdit" class="btn cancel">✖ Cancel</button>
                <button @click="saveChanges" class="btn save" :disabled="saving">💾 Save</button>
            </template>
            <template v-else>
                <button @click="goBack" class="btn back">← Back</button>
                <button v-if="action" @click="enterEditMode" class="btn edit">✏️ Edit</button>
            </template>
        </div>

        <div v-if="action">
            <transition name="fade-slide" mode="out-in">
                <div :key="editMode ? 'edit' : 'view'">
                    <template v-if="editMode && editableAction">
                        <h2>Edit Action</h2>

                        <div class="field">
                            <label>Action Name</label>
                            <input v-model="editableAction.ActionName" />
                        </div>

                        <div class="field">
                            <label>Action Cost</label>
                            <select v-model="editableAction.ActionCostId">
                                <option disabled value="">Select Action Cost</option>
                                <option
                                    v-for="a in chakaischemaStore.getIdNamePairsByTable('action_costs').map(s => ({
                                            name: s.ChakaiSchemaName,
                                            id: s.ChakaiSchemaId
                                        }))"
                                    :key="a.id"
                                    :value="a.id" 
                                >
                                    {{ a.name }}
                                </option>
                            </select>
                        </div>

                        <div class="field">
                            <label>Action Category</label>
                            <select v-model="editableAction.ActionCategoryId">
                                <option disabled value="">Select Action Category</option>
                                <option
                                    v-for="a in chakaischemaStore.getIdNamePairsByTable('action_categories').map(s => ({
                                            name: s.ChakaiSchemaName,
                                            id: s.ChakaiSchemaId
                                        }))"
                                    :key="a.id"
                                    :value="a.id" 
                                >
                                    {{ a.name }}
                                </option>
                            </select>
                        </div>

                        <div class="field">
                            <label>Gameplay Mode</label>
                            <select v-model="editableAction.GameplayModeId">
                                <option disabled value="">Select Gameplay Mode</option>
                                <option
                                    v-for="a in chakaischemaStore.getIdNamePairsByTable('gameplay_modes').map(s => ({
                                            name: s.ChakaiSchemaName,
                                            id: s.ChakaiSchemaId
                                        }))"
                                    :key="a.id"
                                    :value="a.id" 
                                >
                                    {{ a.name }}
                                </option>
                            </select>
                        </div>

                        <div class="field">
                            <label>Description</label>
                            <textarea v-model="editableAction.ActionDescription" rows="6" />
                        </div>
                        
                        <SimpleGrid
                            title="Assigned Traits"
                            :items="action.Traits"
                            idField="TraitId"
                            nameField="TraitName"
                            descriptionField="TraitDescription"
                            @item-clicked="handleActionTraitClicked"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Available Traits"
                            :items="availableTraits"
                            idField="TraitId"
                            nameField="TraitName"
                            descriptionField="TraitDescription"
                            @item-clicked="addTraitToAction"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Assigned Rolls"
                            :items="assignedRolls"
                            idField="ChakaiSchemaId"
                            nameField="ChakaiSchemaName"
                            @item-clicked="handleActionRollClicked"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Available Rolls"
                            :items="availableRolls"
                            idField="ChakaiSchemaId"
                            nameField="ChakaiSchemaName"
                            @item-clicked="addRollToAction"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <!-- Delete Action Button (uses shared btn.delete styling) -->
                        <button
                            @click="deleteModal.open()"
                            class="btn delete"
                            :disabled="saving || deleting"
                            title="Delete action"
                        >
                            🗑️ Delete Action
                        </button>
                    </template>

                    <template v-else>
                        <h2>{{ action.ActionName }} {{ COST_ALIASES[action.ActionCostName] ?? ' - ' + action.ActionCostName }}</h2>
                        <TraitsDisplay :traits="action.Traits || []" />

                        <div class="field">
                            <label>Category</label>
                            <span>{{ action.ActionCategoryName }}</span>
                        </div>

                        <div class="field">
                            <label>Gameplay Mode</label>
                            <span>{{ action.GameplayModeName }}</span>
                        </div>

                        <div class="field">
                            <label>Description</label>
                            <span class="pre">{{ action.ActionDescription }}</span>
                        </div>
                        
                    </template>
                </div>
            </transition>
            
        </div>

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="removeTraitModal"
            message="Are you sure you want to remove this trait from this action?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removeTraitFromAction"
        />

        <ConfirmationModal
            ref="removeRollModal"
            message="Are you sure you want to remove this roll from this action?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removeRollFromAction"
        />

        <!-- Delete Action Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this action? This cannot be undone."
            confirmButtonText="Yes, Delete"
            :onConfirm="confirmDeleteAction"
        />

        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
    </div>
    
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { actionStore, chakaischemaStore, traitStore } from '@/stores';
import api from '@/api';
import { getErrorMessage } from '@/utils/error';
import type { Action, ChakaiSchema, Trait } from '@/types';
import TraitsDisplay from '@/components/TraitsDisplay.vue';
import { COST_ALIASES } from '@/utils/costAliases';

const router = useRouter();
const route = useRoute();
const actionId = Number(route.params.actionId);

const action = computed(() => actionStore.items.find(a => a.ActionId === actionId));
const editMode = ref(false);
const saving = ref(false);
const errorMessage = ref<string | null>(null);
const editableAction = ref<Action | null>(null);

const goBack = () => router.back();

const enterEditMode = () => {
    if (action.value) {
        editableAction.value = { ...action.value };
        editMode.value = true;
    }
};

const cancelEdit = () => {
    editableAction.value = null;
    editMode.value = false;
    errorMessage.value = null;
};

const saveChanges = async () => {
    if (!editableAction.value) return;
    saving.value = true;
    errorMessage.value = null;

    try {
        const response = await api.put(`/actions/${actionId}`, {
            ActionName: editableAction.value.ActionName,
            ActionDescription: editableAction.value.ActionDescription,
            ActionCostId: editableAction.value.ActionCostId,
            ActionCategoryId: editableAction.value.ActionCategoryId,
            GameplayModeId: editableAction.value.GameplayModeId,
            Traits: action.value?.Traits?.map(trait => ({ TraitId: trait.TraitId})),
            Rolls: action.value?.Rolls?.map(roll => ({ RollId: roll.RollId}))
        });

        const index = actionStore.items.findIndex(a => a.ActionId === actionId);
        if (index !== -1) {
            actionStore.items[index] = response.data.result;
        }
        editMode.value = false;
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        saving.value = false;
    }
};

// Computed property to filter out already selected actions from the "choose actions" list
const availableTraits = computed(() => {
    return traitStore.items.filter(trait => 
        !action.value?.Traits.some(t => t.TraitId === trait.TraitId)
    );
});

// Computed property to filter out already selected actions from the "choose actions" list
const availableRolls = computed(() => {
    return chakaischemaStore.getIdNamePairsByTable("rolls").filter(roll => 
        !action.value?.Rolls.some(r => r.RollId === roll.ChakaiSchemaId)
    );
});

const assignedRolls = computed(() => {
    return chakaischemaStore.getIdNamePairsByTable("rolls").filter(roll => 
        action.value?.Rolls.some(r => r.RollId === roll.ChakaiSchemaId)
    );
});

const addTraitToAction = (trait: Trait) => {

    if (!action.value || !action) return;

    const alreadyExists = action.value.Traits?.some(
        (t: Trait) => t.TraitId === trait.TraitId
    );

    if (!alreadyExists) {
        action.value.Traits = [
            ...(action.value.Traits || []),
            trait
        ];
    }
};

const addRollToAction = (roll: ChakaiSchema) => {

    if (!action.value || !action) return;

    const alreadyExists = action.value.Rolls?.some(
        (r) => r.RollId === roll.ChakaiSchemaId
    );

    if (!alreadyExists) {
        action.value.Rolls = [
            ...(action.value.Rolls || []),
            {RollId: roll.ChakaiSchemaId}
        ];
    }
};

const removeTraitModal = ref();
const removeRollModal = ref();
const deleteModal = ref();
const traitToRemove = ref<Trait | null>(null);
const rollToRemove = ref<ChakaiSchema | null>(null);
const deleting = ref(false);

const handleActionTraitClicked = (trait: Trait) => {
    traitToRemove.value = trait;
    removeTraitModal.value?.open();
};

const handleActionRollClicked = (roll: ChakaiSchema) => {
    rollToRemove.value = roll;
    removeRollModal.value?.open();
};

const removeTraitFromAction = () => {

    if (!action.value || !traitToRemove) return;

    action.value.Traits = action.value.Traits.filter(a => a.TraitId !== traitToRemove.value!.TraitId);
}

const removeRollFromAction = () => {

    if (!action.value || !rollToRemove) return;

    action.value.Rolls = action.value.Rolls.filter(a => a.RollId !== rollToRemove.value!.ChakaiSchemaId);
}

const confirmDeleteAction = async () => {
    if (!action.value) return;
    deleting.value = true;
    errorMessage.value = null;
    try {
        await api.delete(`/actions/${actionId}`);
        const idx = actionStore.items.findIndex(a => a.ActionId === actionId);
        if (idx !== -1) {
            actionStore.items.splice(idx, 1);
        }
        deleteModal.value?.close();
        router.push('/actions');
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        deleting.value = false;
    }
}

</script>

<style scoped>
.fade-slide-enter-active,
.fade-slide-leave-active {
    transition: all 0.3s ease;
}

.fade-slide-enter-from,
.fade-slide-leave-to {
    opacity: 0;
    transform: translateY(10px);
}
</style>
