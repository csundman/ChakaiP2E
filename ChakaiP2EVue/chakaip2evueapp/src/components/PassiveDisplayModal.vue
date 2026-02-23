<template>
<div v-if="showDescriptionModal" class="description-modal-overlay" @click="close">
    <div class="description-modal-content" @click.stop>
        <h3>{{ displayPassive?.PassiveName }}</h3>
        <TraitsDisplay :traits="displayPassive?.Traits || []" />
        <p>{{ displayPassive?.PassiveDescription }}</p>
    </div>
</div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import TraitsDisplay from '@/components/TraitsDisplay.vue';
import RollsDisplay from './RollsDisplay.vue';
import { COST_ALIASES } from '@/utils/costAliases';
import type { Passive } from '@/types';

const showDescriptionModal = ref(false);
const displayPassive = ref<Passive | null>(null);

const open = (passive: Passive) => {
    displayPassive.value = passive;
    showDescriptionModal.value = true;
};

// Function to close the modal
const close = () => {
    showDescriptionModal.value = false;
};

defineExpose({ open, close });

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
  padding: 20px;
  max-height: 80%;
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

  .description-modal-content h3 {
    color: #f5f5f5;
  }

  .description-modal-content p {
    color: #dcdcdc;
  }
}


</style>