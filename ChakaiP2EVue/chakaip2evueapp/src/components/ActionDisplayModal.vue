<template>
<div v-if="showDescriptionModal" class="description-modal-overlay" @click="close">
    <div class="description-modal-content" @click.stop>
        <h2>{{ displayAction?.ActionName }} {{ displayActionCost }}</h2>
        <TraitsDisplay :traits="displayAction?.Traits || []" />
        <p>{{ displayAction?.ActionDescription }}</p>
        <h2>Associated Passives:</h2>
        <div v-if="associatedPassives.length === 0" class="no-passives">
            None
        </div>

        <div class="passive-list">
            <div 
                class="passive-card"
                v-for="passive in associatedPassives"
                :key="passive.PassiveId"
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
import TraitsDisplay from '@/components/TraitsDisplay.vue';
import RollsDisplay from './RollsDisplay.vue';
import { COST_ALIASES } from '@/utils/costAliases';
import type { Action, Passive } from '@/types';

const showDescriptionModal = ref(false);
const displayAction = ref<Action | null>(null);
const associatedPassives = ref<Passive[]>([]);

const open = (action: Action, passives: Passive[]) => {
    displayAction.value = action;
    showDescriptionModal.value = true;
    associatedPassives.value = passives;
};

// Function to close the modal
const close = () => {
    associatedPassives.value = [];
    showDescriptionModal.value = false;
};

defineExpose({ open, close });

// Computed display string for the selected action's cost (uses alias map)
const displayActionCost = computed(() => {
    if (!displayAction.value) return '';
    const name = displayAction.value.ActionCostName ?? '';
    return COST_ALIASES[name] ?? name;
});

</script>

<style scoped>

.description-modal-overlay {
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

.description-modal-content {
  background-color: white;
  max-height: 80%;
  padding: 20px;
  max-width: 800px;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  overflow-y: auto;
}

.description-modal-content p {
  font-size: 1.2rem;
  margin: 0;
  white-space: pre-wrap;
}

.passive-list {
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.passive-card {
  padding: 12px 14px;
  background: #f8f8f8;
  border-radius: 6px;
  border-left: 4px solid #4b8be0;
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

.no-passives {
  font-style: italic;
  color: #777;
  margin-top: 8px;
}

/* -----------------------------
   DARK MODE
   ----------------------------- */
@media (prefers-color-scheme: dark) {
  .description-modal-overlay {
    background-color: rgba(0, 0, 0, 0.7);
  }

  .description-modal-content {
    background-color: #2a2a2a;
    color: #e0e0e0;
    box-shadow: 0 0 15px rgba(0, 0, 0, 0.8);
  }

  .description-modal-content h2 {
    color: #f5f5f5;
  }

  .description-modal-content p {
    color: #dcdcdc;
  }

  .passive-card {
    background-color: #3a3a3a;
    border-left-color: #4b8be0; /* keep accent color */
  }

  .passive-name {
    color: #ffffff;
  }

  .passive-description {
    color: #c0c0c0;
  }

  .no-passives {
    color: #999;
  }
}



</style>