<template>
    <div class="panel">
        <div class="top-bar">
            <template v-if="editMode">
                <button @click="cancelEdit" class="btn cancel">✖ Cancel</button>
                <button @click="saveChanges" class="btn save" :disabled="saving">💾 Save</button>
            </template>
            <template v-else>
                <button @click="goBack" class="btn back">← Back</button>
                <button v-if="passive" @click="enterEditMode" class="btn edit">✏️ Edit</button>
            </template>
        </div>

        <div v-if="passive">
            <transition name="fade-slide" mode="out-in">
                <div :key="editMode ? 'edit' : 'view'">
                    <template v-if="editMode && editablePassive">
                        <h2>Edit Passive</h2>

                        <div class="field">
                            <label>Passive Name</label>
                            <input v-model="editablePassive.PassiveName" />
                        </div>

                        <div class="field">
                            <label>Passive Type</label>
                            <select v-model="editablePassive.PassiveTypeId">
                                <option disabled value="">Select Passive Type</option>
                                <option
                                    v-for="a in chakaischemaStore.getIdNamePairsByTable('passive_types').map(s => ({
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
                            <textarea v-model="editablePassive.PassiveDescription" rows="6" />
                        </div>

                        <SimpleGrid
                            title="Assigned Traits"
                            :items="passive.Traits"
                            idField="TraitId"
                            nameField="TraitName"
                            descriptionField="TraitDescription"
                            @item-clicked="handlePassiveTraitClicked"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Available Traits"
                            :items="availableTraits"
                            idField="TraitId"
                            nameField="TraitName"
                            descriptionField="TraitDescription"
                            @item-clicked="addTraitToPassive"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Assigned Rolls"
                            :items="assignedRolls"
                            idField="ChakaiSchemaId"
                            nameField="ChakaiSchemaName"
                            @item-clicked="handlePassiveRollClicked"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Available Rolls"
                            :items="availableRolls"
                            idField="ChakaiSchemaId"
                            nameField="ChakaiSchemaName"
                            @item-clicked="addRollToPassive"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Assigned Actions"
                            :items="passive.Actions"
                            idField="ActionId"
                            nameField="ActionName"
                            @item-clicked="handlePassiveActionClicked"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <SimpleGrid
                            title="Available Actions"
                            :items="availableActions"
                            idField="ActionId"
                            nameField="ActionName"
                            @item-clicked="addActionToPassive"
                            :errorMessage="null"
                            :pageSize="10"
                        />

                        <button
                            @click="deleteModal.open()"
                            class="btn delete"
                            :disabled="saving || deleting"
                            title="Delete passive"
                        >
                            🗑️ Delete Passive
                        </button>
                    </template>

                    <template v-else>
                        <h2>{{ passive.PassiveName }}</h2>
                        <TraitsDisplay :traits="passive.Traits || []" />

                        <div class="field">
                            <label>Type</label>
                            <span>{{ passive.PassiveTypeName }}</span>
                        </div>

                        <div class="field">
                            <label>Description</label>
                            <span class="pre">{{ passive.PassiveDescription }}</span>
                        </div>
                        
                    </template>
                </div>
            </transition>
            
        </div>

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="removeTraitModal"
            message="Are you sure you want to remove this trait from this passive?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removeTraitFromPassive"
        />

        <ConfirmationModal
            ref="removeRollModal"
            message="Are you sure you want to remove this roll from this passive?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removeRollFromPassive"
        />

        <ConfirmationModal
            ref="removeActionModal"
            message="Are you sure you want to remove this action from this passive?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removeActionFromPassive"
        />

        <!-- Delete Passive Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this passive? This cannot be undone."
            confirmButtonText="Yes, Delete"
            :onConfirm="confirmDeletePassive"
        />

        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
    </div>
    
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { passiveStore, chakaischemaStore, traitStore, actionStore } from '@/stores';
import api from '@/api';
import { getErrorMessage } from '@/utils/error';
import type { Passive, Trait, ChakaiSchema, Action } from '@/types';
import TraitsDisplay from '@/components/TraitsDisplay.vue';

const router = useRouter();
const route = useRoute();
const passiveId = Number(route.params.passiveId);

const passive = computed(() => passiveStore.items.find(a => a.PassiveId === passiveId));
const editMode = ref(false);
const saving = ref(false);
const errorMessage = ref<string | null>(null);
const editablePassive = ref<Passive | null>(null);

const goBack = () => router.back();

const enterEditMode = () => {
    if (passive.value) {
        editablePassive.value = { ...passive.value };
        editMode.value = true;
    }
};

const cancelEdit = () => {
    editablePassive.value = null;
    editMode.value = false;
    errorMessage.value = null;
};

const saveChanges = async () => {
    if (!editablePassive.value) return;
    saving.value = true;
    errorMessage.value = null;

    try {
        const response = await api.put(`/passives/${passiveId}`, {
            PassiveName: editablePassive.value.PassiveName,
            PassiveDescription: editablePassive.value.PassiveDescription,
            PassiveTypeId: editablePassive.value.PassiveTypeId,
            Traits: passive.value?.Traits?.map(trait => ({ TraitId: trait.TraitId})),
            Rolls: passive.value?.Rolls?.map(roll => ({ RollId: roll.RollId})),
            Actions: passive.value?.Actions?.map(action => ({ ActionId: action.ActionId})),
        });

        const index = passiveStore.items.findIndex(a => a.PassiveId === passiveId);
        if (index !== -1) {
            passiveStore.items[index] = response.data.result;
        }
        editMode.value = false;
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        saving.value = false;
    }
};

const removeTraitModal = ref();
const removeRollModal = ref();
const removeActionModal = ref();
const deleteModal = ref();
const traitToRemove = ref<Trait | null>(null);
const rollToRemove = ref<ChakaiSchema | null>(null);
const actionToRemove = ref<Action | null>(null);
const deleting = ref(false);

const handlePassiveTraitClicked = (trait: Trait) => {
    traitToRemove.value = trait;
    removeTraitModal.value?.open();
};

const handlePassiveRollClicked = (roll: ChakaiSchema) => {
    rollToRemove.value = roll;
    removeRollModal.value?.open();
};

const handlePassiveActionClicked = (action: Action) => {
    actionToRemove.value = action;
    removeActionModal.value?.open();
};

const removeTraitFromPassive = () => {

    if (!passive.value || !traitToRemove) return;

    passive.value.Traits = passive.value.Traits.filter(a => a.TraitId !== traitToRemove.value!.TraitId);
}

const removeRollFromPassive = () => {

    if (!passive.value || !rollToRemove) return;

    passive.value.Rolls = passive.value.Rolls.filter(a => a.RollId !== rollToRemove.value!.ChakaiSchemaId);
}

const removeActionFromPassive = () => {

    if (!passive.value || !actionToRemove) return;

    passive.value.Actions = passive.value.Actions.filter(a => a.ActionId !== actionToRemove.value!.ActionId);
}

const confirmDeletePassive = async () => {
    if (!passive.value) return;
    deleting.value = true;
    errorMessage.value = null;
    try {
        await api.delete(`/passives/${passiveId}`);
        const idx = passiveStore.items.findIndex(a => a.PassiveId === passiveId);
        if (idx !== -1) {
            passiveStore.items.splice(idx, 1);
        }
        deleteModal.value?.close();
        router.push('/passives');
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        deleting.value = false;
    }
}

// Computed property to filter out already selected actions from the "choose actions" list
const availableTraits = computed(() => {
    return traitStore.items.filter(trait => 
        !passive.value?.Traits.some(t => t.TraitId === trait.TraitId)
    );
});

// Computed property to filter out already selected actions from the "choose actions" list
const availableRolls = computed(() => {
    return chakaischemaStore.getIdNamePairsByTable("rolls").filter(roll => 
        !passive.value?.Rolls.some(r => r.RollId === roll.ChakaiSchemaId)
    );
});

const assignedRolls = computed(() => {
    return chakaischemaStore.getIdNamePairsByTable("rolls").filter(roll => 
        passive.value?.Rolls.some(r => r.RollId === roll.ChakaiSchemaId)
    );
});

// Computed property to filter out already selected actions from the "choose actions" list
const availableActions = computed(() => {
    return actionStore.items.filter(action => 
        !passive.value?.Actions.some(a => a.ActionId === action.ActionId)
    );
});

const addTraitToPassive = (trait: Trait) => {

    if (!passive.value || !passive) return;

    const alreadyExists = passive.value.Traits?.some(
        (t: Trait) => t.TraitId === trait.TraitId
    );

    if (!alreadyExists) {
        passive.value.Traits = [
            ...(passive.value.Traits || []),
            trait
        ];
    }
};

const addRollToPassive = (roll: ChakaiSchema) => {

    if (!passive.value || !passive) return;

    const alreadyExists = passive.value.Rolls?.some(
        (r) => r.RollId === roll.ChakaiSchemaId
    );

    if (!alreadyExists) {
        passive.value.Rolls = [
            ...(passive.value.Rolls || []),
            {RollId: roll.ChakaiSchemaId}
        ];
    }
};

const addActionToPassive = (action: Action) => {

    if (!passive.value || !passive) return;

    const alreadyExists = passive.value.Actions?.some(
        (a: Action) => a.ActionId === action.ActionId
    );

    if (!alreadyExists) {
        passive.value.Actions = [
            ...(passive.value.Actions || []),
            action
        ];
    }
};

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
