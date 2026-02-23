<template>
    <div class="characters-page">
        <h1>Characters</h1>

        <!-- Floating + Button -->
        <button class="btn add" @click="handleButtonAddCharacter">Add Character</button>
        
        <div v-if="characterStore.loading" class="loading">Loading...</div>
        <div v-if="characterStore.errorMessage" class="error-message">
            Error: {{ characterStore.errorMessage }}
        </div>
  
        <!-- Character Grid -->
        <div class="character-grid" v-if="characterStore.items.length > 0">
            <div
                class="character-card"
                v-for="character in characterStore.items"
                :key="character.CharacterId"
                @click="goToCharacterDetails(character.CharacterId)"
            >
                <div class="character-name">{{ character.CharacterName }}</div>
                <div class="character-stats">
                    <div><strong>Ancestry:</strong> {{ character.AncestryName }}</div>
                    <div><strong>Background:</strong> {{ character.BackgroundName }}</div>
                    <div><strong>Class:</strong> {{ character.ClassName }}</div>
                </div>
            </div>
        </div>

        <!-- Modal for adding a new character -->
        
        <ModalCreateForm 
            ref="addModal"
            title="Create New Action"
            :fields="characterFormFields" 
            :onSubmit="handleSubmitAddCharacter"
            :errorMessage="submitErrorMessage" />

    </div>
</template>
  
<script setup lang="ts">
    import { ref, onMounted, computed } from 'vue';
    import api from '../api.ts';
    import { getErrorMessage } from '../utils/error.ts';
    import { ancestryStore, backgroundStore, cclassStore, characterStore } from '@/stores/'
    import type { Ancestry, Background, CClass, Character } from '@/types/';
    import { useRouter } from 'vue-router';
    const router = useRouter();
    
    const addModal = ref();
    const submitErrorMessage = ref<string | null>(null);

    // --- FORM DEFINITION ---
    const characterFormFields = computed(() => [
        {
            name: 'CharacterName',
            label: 'Character Name',
            type: 'string',
            placeholder: 'Enter character name',
            required: true,
            value: ''
        },
        {
            name: 'AncestryId',
            label: 'Ancestry',
            type: 'searchable-select',
            required: false,
            options: ancestryStore.items.map(a => ({
                label: a.AncestryName,
                value: a.AncestryId
            })),
            value: ''
        },
        {
            name: 'BackgroundId',
            label: 'Background',
            type: 'searchable-select',
            required: false,
            options: backgroundStore.items.map(a => ({
                label: a.BackgroundName,
                value: a.BackgroundId
            })),
            value: ''
        },
        {
            name: 'CClassId',
            label: 'Class',
            type: 'searchable-select',
            required: false,
            options: cclassStore.items.map(a => ({
                label: a.ClassName,
                value: a.CClassId
            })),
            value: ''
        }
    ]);

    // Create new character
    const handleSubmitAddCharacter = async (formData: Record<string, any>) => {
        if (!formData.CharacterName?.trim()) return;

        try {
            const rawPayload = {
                CharacterName: formData.CharacterName.trim(),
                AncestryId: formData.AncestryId,
                BackgroundId: formData.BackgroundId,
                CClassId: formData.CClassId
            };

            // Filter out properties with null/undefined/empty string values
            const payload = Object.fromEntries(
                Object.entries(rawPayload).filter(([_, value]) => value !== '' && value != null)
            );

            const response = await api.post('/characters/', payload);

            await characterStore.forceLoad();
            addModal.value.close();
        } catch (error) {
            console.error("Error creating character:", error);
        }
    };

    const goToCharacterDetails = (characterId: number) => {
        router.push(`/characters/${characterId}`);
    };

    const handleButtonAddCharacter = () => {
        submitErrorMessage.value = null;
        addModal.value.open();
    };

</script>

    
<style scoped>
  
    .loading {
        text-align: center;
        font-size: 1.5em;
        color: #888;
    }
  
    .character-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
        gap: 20px;
        padding: 10px;
    }
  
    .character-card {
        position: relative;  /* Added this line */
        background-color: #f9f9f9;
        border: 1px solid #ddd;
        border-radius: 8px;
        padding: 15px;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
    }
    
    .character-name {
        font-size: 1.3em;
        font-weight: bold;
        color: #333;
        margin-bottom: 10px;
    }
    
    .character-stats {
        font-size: 1em;
        color: #555;
    }
    
    .character-stats div {
        margin-bottom: 5px;
    }

    .character-card {
        cursor: pointer;
        transition: transform 0.2s ease;
    }
    .character-card:hover {
        transform: scale(1.02);
    }

    @media (prefers-color-scheme: dark) {
        .loading {
            color: #bbb;
        }

        .character-card {
            background-color: #2a2a2a;
            border-color: #333;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.5);
        }

        .character-name {
            color: #eee;
        }

        .character-stats {
            color: #ccc;
        }
    }
    
</style>
