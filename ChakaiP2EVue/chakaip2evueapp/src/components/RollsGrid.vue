<template>
  <div class="rolls-grid-container">
    <div
      class="roll-box"
      v-for="roll in sortedRolls"
      :key="roll.RollId"
      @click="handleRollClicked(roll)"
    >
      <div class="roll-name" >{{ displayName(roll) }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import type { Ref } from 'vue';

type Roll = {
    RollId: number;
    RollName: string;
    RollShortName?: string | null;
};

const props = defineProps<{ rolls: Roll[] }>();

const emit = defineEmits<{ 
    (e: 'roll-clicked', RollId: any): void; 
}>();

const isMobile: Ref<boolean> = ref(window.innerWidth < 768);
const updateIsMobile = () => (isMobile.value = window.innerWidth < 768);
onMounted(() => window.addEventListener('resize', updateIsMobile));
onBeforeUnmount(() => window.removeEventListener('resize', updateIsMobile));

// Sort alphabetically by ChakaiSchemaName (case-insensitive)
const sortedRolls = computed(() => {
  return [...(props.rolls || [])].sort((a, b) =>
    a.RollName.localeCompare(b.RollName, undefined, { sensitivity: 'base' })
  );
});

const displayName = (roll: Roll) => {
  if (isMobile.value && roll.RollShortName) return roll.RollShortName;
  return roll.RollName;
};

const handleRollClicked = (roll: Roll) => {
  emit('roll-clicked', roll.RollId);
};

</script>

<style scoped>
.rolls-grid-container {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  padding: 8px;
  align-items: start;
}

.roll-box {
  background: linear-gradient(135deg, #ffffff 0%, #eef7ff 60%);
  border: 2px solid #3b82f6;
  border-radius: 12px;
  padding: 10px 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  min-height: 56px;
  box-shadow: 0 8px 24px rgba(59, 130, 246, 0.10);
  transition: transform 0.16s ease, box-shadow 0.16s ease, background-color 0.18s ease;
}

.roll-box:hover {
  transform: translateY(-6px);
  box-shadow: 0 18px 44px rgba(59, 130, 246, 0.16);
}

.roll-name {
  font-weight: 600;
  color: #0b2548;
  font-size: 1rem;
}

@media (max-width: 900px) {
  .rolls-grid-container {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 480px) {
  .rolls-grid-container {
    grid-template-columns: repeat(2, 1fr);
  }
}

/* Dark mode */
@media (prefers-color-scheme: dark) {
  .rolls-grid-container {
    gap: 14px;
  }
  .roll-box {
    background: linear-gradient(135deg, #071426 0%, #0b2b3f 60%);
    border: 2px solid rgba(96, 165, 250, 0.16);
    box-shadow: 0 12px 38px rgba(2, 6, 23, 0.6);
  }
  .roll-name {
    color: #e6f0ff;
  }
}

</style>
