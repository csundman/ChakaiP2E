<template>
<div v-if="showReminderModal" class="reminder-modal-overlay" @click="close">
    <div class="reminder-modal-content" @click.stop>

        <h2>Roll Reminders for {{ displayRoll?.ChakaiSchemaName }}</h2>

        <p v-if="applicableActions && applicableActions.length === 0 && applicablePassives && applicablePassives.length === 0">There are no reminders for this roll</p>

        <p v-if="applicableActions && applicableActions.length > 0">Are you trying to do one of these?</p>

        <div v-if="applicableActions && applicableActions.length > 0" class="reminder-section">
            <div 
                v-for="action in applicableActions" 
                :key="action.ActionId"
                class="reminder-card action-card"
                @click="openActionFromReminder(action)"
            >
                {{ action.ActionName }}
            </div>
        </div>

        <p v-if="applicablePassives && applicablePassives.length > 0">Don't forget:</p>

        <div v-if="applicablePassives && applicablePassives.length > 0" class="reminder-section">
            <div 
                v-for="passive in applicablePassives" 
                :key="passive.PassiveId"
                class="reminder-card passive-card"
            >
                <div class="passive-name">{{ passive.PassiveName }}</div>
                <div class="passive-description">{{ passive.PassiveDescription }}</div>
            </div>

        </div>

    </div>
</div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import type { Action, ChakaiSchema, Character } from '@/types';

const showReminderModal = ref(false);
const displayRoll = ref<ChakaiSchema | null>(null);
const currentCharacter = ref<Character | null>(null);

const open = (roll: ChakaiSchema, character: Character) => {
    displayRoll.value = roll;
    currentCharacter.value = character;
    showReminderModal.value = true;
};

const emit = defineEmits(["action-selected"]);

const openActionFromReminder = (action: Action) => {
    emit("action-selected", action);   // send the action to parent
    close();                           // close reminder modal
};


// Function to close the modal
const close = () => {
    showReminderModal.value = false;
};

defineExpose({ open, close });

const applicableActions = computed(() => {
    return currentCharacter.value?.Actions.filter(action => 
        action.Rolls.some(r => r.RollId === displayRoll.value?.ChakaiSchemaId)
    ) || [];
});

const applicablePassives = computed(() => {
    return currentCharacter.value?.Passives.filter(passive => 
        passive.Rolls.some(r => r.RollId === displayRoll.value?.ChakaiSchemaId)
    ) || [];
});

</script>

<style scoped>

.reminder-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.reminder-modal-content {
  background-color: white;
  padding: 20px;
  max-height: 80%;
  max-width: 800px;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  overflow-y: auto;
}

.reminder-modal-content p {
  font-size: 1.2rem;
  margin: 0;
  white-space: pre-wrap;
}

.reminder-section {
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.reminder-card {
  padding: 12px 14px;
  background: #f8f8f8;
  border-radius: 6px;
  font-size: 1.05rem;
}

.action-card {
  border-left: 4px solid #4b8be0; /* Same as action styling */
}

.passive-card {
  border-left: 4px solid #8c5dd8; /* Slightly different accent */
}

.reminder-modal-content p {
  font-size: 1.2rem;
  margin: 0;
  margin-top: 10px;
  margin-bottom: 5px;
  white-space: pre-wrap;
}

.passive-name {
  font-weight: 600;
  font-size: 1.05rem;
}

.passive-description {
  font-size: 0.95rem;
  margin-top: 4px;
  white-space: pre-wrap;
  color: #444;
}


/* -----------------------------
   DARK MODE
   ----------------------------- */
@media (prefers-color-scheme: dark) {
  .reminder-modal-overlay {
    background-color: rgba(0, 0, 0, 0.7);
  }

  .reminder-modal-content {
    background-color: #2a2a2a;
    color: #e0e0e0;
    box-shadow: 0 0 15px rgba(0, 0, 0, 0.8);
  }

  .reminder-modal-content p {
    color: #dcdcdc;
  }

  .reminder-card {
    background-color: #3a3a3a;
    color: #f0f0f0;
  }

  .action-card {
    border-left-color: #4b8be0;
  }

  .passive-card {
    border-left-color: #8c5dd8;
  }

  .passive-description {
  color: #d0d0d0;
}

}


</style>