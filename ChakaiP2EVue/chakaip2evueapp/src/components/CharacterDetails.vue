<template>
    <div class="character-details">

        <div class="top-bar">
            <template v-if="editMode">
                <button @click="cancelEdit" class="btn cancel">✖ Cancel</button>
                <button @click="saveChanges" class="btn save" :disabled="saving">💾 Save</button>
            </template>
            <template v-else>
                <button @click="goBack" class="btn back">← Back</button>
                <button v-if="character" @click="enterEditMode" class="btn edit">✏️ Edit</button>
            </template>
        </div>

        <div v-if="character">
            <transition name="slide-fade" mode="out-in">
                <div :key="editMode ? 'edit' : 'view'">
                    <template v-if="editMode && editableCharacter">
                        <h2>Edit Character</h2>

                        <div class="detail-field">
                            <label>Character Name:</label>
                            <input v-model="editableCharacter.CharacterName" />
                        </div>

                        <div class="detail-field">
                            <label>Ancestry:</label>
                            <select v-model="editableCharacter.AncestryId">
                                <option disabled value="">Select Ancestry</option>
                                <option
                                    v-for="a in ancestryStore.items"
                                    :key="a.AncestryId"
                                    :value="a.AncestryId"
                                >
                                    {{ a.AncestryName }}
                                </option>
                            </select>
                        </div>

                        <div class="detail-field">
                            <label>Background:</label>
                            <select v-model="editableCharacter.BackgroundId">
                                <option disabled value="">Select Background</option>
                                <option
                                    v-for="b in backgroundStore.items"
                                    :key="b.BackgroundId"
                                    :value="b.BackgroundId"
                                >
                                    {{ b.BackgroundName }}
                                </option>
                            </select>
                        </div>

                        <div class="detail-field">
                            <label>Class:</label>
                            <select v-model="editableCharacter.CClassId">
                                <option disabled value="">Select Class</option>
                                <option
                                    v-for="c in cclassStore.items"
                                    :key="c.CClassId"
                                    :value="c.CClassId"
                                >
                                    {{ c.ClassName }}
                                </option>
                            </select>
                        </div>

                        <button
                            @click="deleteModal.open()"
                            class="btn delete"
                            :disabled="saving || deleting"
                            title="Delete Character"
                        >
                            🗑️ Delete Character
                        </button>
                    </template>

                    <template v-else>
                        <div class="heading-row">
                            <h2>{{ character.CharacterName }} -- {{ character.AncestryName }} {{ character.ClassName }} </h2>
                        </div>

                        <!-- Add the ActionsGrid component below -->
                        <ActionsGrid 
                            :title="'Assigned Actions'"
                            :actions="character.Actions"
                            :error-message=null
                            @action-clicked="handleCharacterActionClicked" />

                        <button 
                            v-if="!showAddActions"
                            class="btn add" 
                            @click="showAddActions = true">
                            ✏️ Edit Actions
                        </button>

                        <button
                            v-if="showAddActions" 
                            class="btn save" @click="saveActions">
                            💾 Save Changes
                        </button>
                        <div style="margin: 10px;"></div>
                        <!-- Add ActionsGrid for choosing -->
                        <ActionsGrid
                            v-if="showAddActions"
                            :title="'Available Actions'"
                            :actions="availableActions"
                            :error-message="null"
                            @action-clicked="addActionToCharacter"
                        />
                        
                        <div class="detail-field">
                            <label>Rolls:</label>
                        </div>

                        <RollsGrid
                            :rolls="chakaischemaStore.getIdNamePairsByTable('rolls').map(s => ({
                                RollId: s.ChakaiSchemaId,
                                RollName: s.ChakaiSchemaName,
                                RollShortName: s.ChakaiSchemaShortName
                            }))"
                            @roll-clicked="handleRollClicked"
                        />

                        <PassivesGrid
                            :title="'Assigned Passives'"
                            :passives="character.Passives"
                            :errorMessage="null"
                            @passive-clicked="handleCharacterPassiveClicked"
                        />

                        <button 
                            v-if="!showAddPassives"
                            class="btn add" 
                            @click="showAddPassives = true">
                            ✏️ Edit Passives
                        </button>

                        <button
                            v-if="showAddPassives" 
                            class="btn save" @click="savePassives">
                            💾 Save Changes
                        </button>
                        <div style="margin: 10px;"></div>
                        <PassivesGrid
                            v-if="showAddPassives"
                            :title="'Available Passives'"
                            :passives="availablePassives"
                            :error-message="null"
                            @passive-clicked="addPassiveToCharacter"
                        />

                    </template>
                </div>
            </transition>
        </div>

        <div v-else>
            <p>Loading character details...</p>
        </div>

        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="deleteModal"
            message="Are you sure you want to delete this character?"
            confirmButtonText="Yes, Delete"
            :onConfirm="deleteCharacter"
        />

        <ActionDisplayModal 
            ref="actionModal"
        />

        <PassiveDisplayModal 
            ref="passiveModal"
        />

        <RollReminderModal 
            ref="rollReminderModal"
            @action-selected="handleCharacterActionClicked"
        />

        <!-- Delete Confirmation Modal -->
        <ConfirmationModal
            ref="removeActionModal"
            message="Are you sure you want to remove this action from this character?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removeActionFromCharacter"
        />

        <ConfirmationModal
            ref="removePassiveModal"
            message="Are you sure you want to remove this passive from this character?"
            confirmButtonText="Yes, Remove"
            :onConfirm="removePassiveFromCharacter"
        />
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { characterStore, ancestryStore, backgroundStore, cclassStore, actionStore, chakaischemaStore, passiveStore } from '@/stores/';
import api from '@/api';
import { getErrorMessage } from '@/utils/error';
import ActionsGrid from '@/components/ActionsGrid.vue';
import PassivesGrid from './PassivesGrid.vue'; 
import TraitsDisplay from '@/components/TraitsDisplay.vue';
import RollsGrid from './RollsGrid.vue';
import ActionDisplayModal from './ActionDisplayModal.vue';
import PassiveDisplayModal from './PassiveDisplayModal.vue';
import ConfirmationModal from '@/components/common/ConfirmationModal.vue';
import RollReminderModal from './RollReminderModal.vue';
import { COST_ALIASES } from '@/utils/costAliases';
import type { Character, Action, Passive } from '@/types';

const router = useRouter();
const route = useRoute();
const characterId = Number(route.params.characterId);

const character = computed(() =>
    characterStore.items.find(c => c.CharacterId === characterId)
);

const editMode = ref(false);
const saving = ref(false);
const deleting = ref(false);
const errorMessage = ref<string | null>(null);
const editableCharacter = ref<Character | null>(null);
const showAddActions = ref(false);
const showAddPassives = ref(false);
const deleteModal = ref();
const actionModal = ref();
const passiveModal = ref();
const removeActionModal = ref();
const removePassiveModal = ref();
const rollReminderModal = ref();

const goBack = () => router.back();

const enterEditMode = () => {
    if (character.value) {
        editableCharacter.value = {
            ...character.value
        };
        editMode.value = true;
    }
};

const addActionToCharacter = (action: Action) => {

    if (!character.value || !action) return;

    const alreadyExists = character.value.Actions?.some(
        (a: Action) => a.ActionId === action.ActionId
    );

    actionsHaveChanged.value = true;

    if (!alreadyExists) {
        character.value.Actions = [
            ...(character.value.Actions || []),
            action
        ];
    }
};

const addPassiveToCharacter = (passive: Passive) => {

    if (!character.value || !passive) return;

    const alreadyExists = character.value.Passives?.some(
        (a: Passive) => a.PassiveId === passive.PassiveId
    );

    passivesHaveChanged.value = true;

    if (!alreadyExists) {
        character.value.Passives = [
            ...(character.value.Passives || []),
            passive
        ];
    }
};

const actionToRemove = ref<Action | null>(null);
const passiveToRemove = ref<Passive | null>(null);
const actionsHaveChanged = ref(false);
const passivesHaveChanged = ref(false);

const handleCharacterActionClicked = (action: Action) => {
    if(showAddActions.value) {
        actionToRemove.value = action;
        removeActionModal.value.open();
    }
    else {
        const applicable = (character.value?.Passives ?? []).filter(p =>
            p.Actions.some(a => a.ActionId === action.ActionId)
        );

        actionModal.value.open(action, applicable);
    }
};

const handleCharacterPassiveClicked = (passive: Passive) => {
    if(showAddPassives.value) {
        passiveToRemove.value = passive;
        removePassiveModal.value.open();
    }
    else {
        passiveModal.value.open(passive)
    }
};

const removeActionFromCharacter = () => {
    if(!showAddActions.value) return;

    if (!character.value || !actionToRemove) return;

    actionsHaveChanged.value = true;
    character.value.Actions = character.value.Actions.filter(a => a.ActionId !== actionToRemove.value!.ActionId);
}

const removePassiveFromCharacter = () => {
    if(!showAddPassives.value) return;

    if (!character.value || !passiveToRemove) return;

    passivesHaveChanged.value = true;
    character.value.Passives = character.value.Passives.filter(a => a.PassiveId !== passiveToRemove.value!.PassiveId);
}

// Computed property to filter out already selected actions from the "choose actions" list
const availableActions = computed(() => {
    return actionStore.items.filter(action => 
        !character.value?.Actions.some(a => a.ActionId === action.ActionId)
    );
});

const availablePassives = computed(() => {
    return passiveStore.items.filter(passive => 
        !character.value?.Passives.some(a => a.PassiveId === passive.PassiveId)
    );
});

const saveActions = async () => {
    showAddActions.value = false;

    if (!actionsHaveChanged.value) return;

    try {
        const response = await api.put(`/characters/${characterId}`, {
            Actions: character.value?.Actions?.map(action => ({ ActionId: action.ActionId}))
        });

        const index = characterStore.items.findIndex(c => c.CharacterId === characterId);
        if (index !== -1) {
            characterStore.items[index] = response.data.result;
        }
        editMode.value = false;
        actionsHaveChanged.value = false;
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        saving.value = false;
    }
}

const savePassives = async () => {
    showAddPassives.value = false;

    if (!passivesHaveChanged.value) return;

    try {
        const response = await api.put(`/characters/${characterId}`, {
            Passives: character.value?.Passives?.map(passive => ({ PassiveId: passive.PassiveId}))
        });

        const index = characterStore.items.findIndex(c => c.CharacterId === characterId);
        if (index !== -1) {
            characterStore.items[index] = response.data.result;
        }
        editMode.value = false;
        passivesHaveChanged.value = false;
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        saving.value = false;
    }
}

const cancelEdit = () => {
    editMode.value = false;
    editableCharacter.value = null;
    errorMessage.value = null;
};

const saveChanges = async () => {
    if (!editableCharacter.value) return;
    saving.value = true;
    errorMessage.value = null;

    try {
        const response = await api.put(`/characters/${characterId}`, {
            CharacterName: editableCharacter.value.CharacterName,
            AncestryId: editableCharacter.value.AncestryId,
            BackgroundId: editableCharacter.value.BackgroundId,
            CClassId: editableCharacter.value.CClassId
        });

        const index = characterStore.items.findIndex(c => c.CharacterId === characterId);
        if (index !== -1) {
            characterStore.items[index] = response.data.result;
        }
        editMode.value = false;
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    } finally {
        saving.value = false;
    }
};

const deleteCharacter = async () => {
    if (character.value) {
        deleting.value = true;
        try {
            await api.delete(`/characters/${character.value.CharacterId}`);
            await characterStore.forceLoad();
        } catch (error) {
            console.error("Error deleting character:", error);
        }
        deleting.value = false;
        router.push('/characters');
    }
};

const handleRollClicked = (rollId: number) => {
    rollReminderModal.value.open(
        chakaischemaStore.getIdNamePairsByTable('rolls').find(r => r.ChakaiSchemaId === rollId),
        character.value);
};

</script>

<style scoped>
.character-details {
    padding: 24px;
    max-width: 600px;
    margin: auto;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background-color: #fafafa;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.character-details h2 {
    margin-bottom: 20px;
    font-size: 1.8rem;
}

.detail-field {
    margin-bottom: 5px;
    display: flex;
    flex-direction: column;
}

.detail-field label {
    font-size: 1.2rem;
    font-weight: 600;
    color: #333;
    margin-bottom: 6px;
    letter-spacing: 0.5px;
}

.detail-field span {
    font-size: 1.4rem;
    padding: 10px 14px;
    background-color: #f8f9fa;
    border: 1px solid #e0e0e0;
    border-radius: 6px;
    color: #444;
    line-height: 1.6;
}

.detail-field input,
.detail-field select {
    font-size: 1.4rem;
    padding: 10px 14px;
    border: 1px solid #ccc;
    border-radius: 6px;
    background-color: #fff;
    transition: border-color 0.2s, background-color 0.2s, box-shadow 0.2s;
}

.detail-field input:focus,
.detail-field select:focus {
    border-color: #4a90e2;
    background-color: #f0f8ff;
    box-shadow: 0 0 0 3px rgba(74, 144, 226, 0.2);
    outline: none;
}


.button-group {
    display: flex;
    gap: 12px;
    margin-top: 20px;
    flex-wrap: wrap;
}

.error {
    color: red;
    margin-top: 20px;
}

/* Slide-fade transition */
.slide-fade-enter-active,
.slide-fade-leave-active {
    transition: all 0.3s ease;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
    opacity: 0;
}

.top-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}

.heading-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 12px;
    margin-bottom: 16px;
}

@media (prefers-color-scheme: dark) {
    .character-details {
        background-color: #1f1f1f;
        color: #f0f0f0;
        box-shadow: 0 2px 10px rgba(255, 255, 255, 0.06);
    }

    .detail-field label {
        color: #ddd;
    }

    .detail-field span {
        background-color: #2c2c2c;
        border-color: #444;
        color: #e0e0e0;
    }

    .detail-field input,
    .detail-field select {
        background-color: #2a2a2a;
        color: #f0f0f0;
        border-color: #555;
    }

    .detail-field input:focus,
    .detail-field select:focus {
        background-color: #333;
        border-color: #6ca0dc;
        box-shadow: 0 0 0 3px rgba(106, 170, 255, 0.2);
    }

    .back-button {
        background-color: #333;
        color: #eee;
    }

    .back-button:hover {
        background-color: #444;
    }

    .button-group button {
        box-shadow: none;
    }

    .error {
        color: #ff9999;
    }

    /* Dark-mode styles for the description modal */
    .description-modal-overlay {
        background-color: rgba(0, 0, 0, 0.72);
    }

    .description-modal-content {
        background-color: #0f1720;
        color: #e6eefc;
        border: 1px solid rgba(120, 160, 255, 0.12);
        box-shadow: 0 12px 36px rgba(2, 6, 23, 0.7);
        transition: background-color 0.22s ease, color 0.22s ease, box-shadow 0.22s ease, border-color 0.22s ease;
    }

    .description-modal-content p {
        color: #d7e6ff;
    }
}

</style>
