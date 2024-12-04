"use client";

import { createContext, useContext, useState, useEffect, ReactNode } from 'react';

interface Character {
  id: number;
  name: string;
  background: string;
  ancestry: string;
  class: string;
}

interface CharacterContextType {
  characters: Character[];
  isLoading: boolean;
  error: string | null;
  refreshCharacters: () => Promise<void>;
}

const CharacterContext = createContext<CharacterContextType | undefined>(undefined);

// Dummy data - move this to a separate file if you prefer
const DUMMY_CHARACTERS: Character[] = [
  { id: 1, name: 'Example Character', background: 'Acolyte', ancestry: 'Human', class: 'Fighter' },
  { id: 2, name: 'Sample Hero', background: 'Criminal', ancestry: 'Elf', class: 'Wizard' },
];

export function CharacterProvider({ children }: { children: ReactNode }) {
  const [characters, setCharacters] = useState<Character[]>(DUMMY_CHARACTERS);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchCharacters = async () => {
    try {
      setIsLoading(true);
      // Replace with actual API call
      const res = await fetch('your-api-endpoint/characters');
      if (!res.ok) {
        throw new Error('Failed to fetch characters');
      }
      const data = await res.json();
      setCharacters(data);
    } catch (err) {
      console.log('Error caught, using dummy data:', DUMMY_CHARACTERS);
      setError(err instanceof Error ? err.message : 'An error occurred');
      setCharacters(DUMMY_CHARACTERS);
    } finally {
      setIsLoading(false);
    }
  };

  // Initial fetch
  useEffect(() => {
    fetchCharacters();
  }, []);

  return (
    <CharacterContext.Provider 
      value={{ 
        characters, 
        isLoading, 
        error,
        refreshCharacters: fetchCharacters
      }}
    >
      {children}
    </CharacterContext.Provider>
  );
}

// Custom hook to use the character context
export function useCharacters() {
  const context = useContext(CharacterContext);
  if (context === undefined) {
    throw new Error('useCharacters must be used within a CharacterProvider');
  }
  return context;
} 