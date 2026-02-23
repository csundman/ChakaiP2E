<template>
  <div class="traits-display-container">
    <div class="trait-box" v-for="trait in traits" :key="trait.TraitId">
      <div class="trait-name">{{ trait.TraitName }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Trait } from '@/types/trait';
const props = defineProps<{ traits: Trait[] }>();
</script>

<style scoped>
.traits-display-container {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  padding: 8px;
}
.trait-box {
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  border: 2px solid #4f8cff;
  border-radius: 14px;
  padding: 4px 10px;
  width: auto;
  max-width: 100%;
  box-shadow: 0 10px 30px rgba(37, 120, 248, 0.14), 0 2px 6px rgba(15, 30, 60, 0.04);
  margin: 0;
  display: inline-flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  word-break: break-word;
  white-space: normal;
  flex: 0 0 auto;
  transition: background-color 0.22s ease, color 0.22s ease, border-color 0.22s ease, box-shadow 0.22s ease, transform 0.16s cubic-bezier(.2,.9,.2,1);
}
.trait-name {
  font-weight: 600;
  font-size: 1.08rem;
  color: #2d3a4a;
  text-align: center;
}
.trait-description {
  font-size: 0.97rem;
  color: #3a4a5a;
  text-align: center;
}
@media (max-width: 600px) {
  .trait-box {
    min-width: unset;
  }
  .traits-display-container {
    gap: 10px;
    padding: 4px;
  }
}
/* Dark mode styles */
@media (prefers-color-scheme: dark) {
  .traits-display-container {
    gap: 14px;
  }

  .trait-box {
    background: linear-gradient(135deg, #3b7dd8 0%, #0a3250 60%, #0f2b44 100%);
    border: 2px solid rgba(95, 160, 255, 0.18);
    box-shadow: 0 16px 42px rgba(2, 6, 23, 0.6), 0 6px 12px rgba(15, 30, 60, 0.08);
    color: #e6eefc;
  }

  .trait-name {
    color: #eaf3ff;
  }

  .trait-description {
    color: #c8d8f8;
  }
}
</style>
