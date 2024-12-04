"use client";

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';

interface Character {
  id: number;
  name: string;
  background: string;
  ancestry: string;
  class: string;
}

const DUMMY_CHARACTERS: Character[] = [
  { id: 1, name: 'Example Character', background: 'Acolyte', ancestry: 'Human', class: 'Fighter' },
  { id: 2, name: 'Sample Hero', background: 'Criminal', ancestry: 'Elf', class: 'Wizard' },
];

export default function CharacterDetails() {
  const params = useParams();
  const [character, setCharacter] = useState<Character | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchCharacter() {
      try {
        // For now, simulate API call with dummy data
        const foundCharacter = DUMMY_CHARACTERS.find(
          c => c.id === Number(params.id)
        );
        
        if (!foundCharacter) {
          throw new Error('Character not found');
        }
        
        setCharacter(foundCharacter);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setIsLoading(false);
      }
    }

    fetchCharacter();
  }, [params.id]);

  if (isLoading) return <div>Loading character...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!character) return <div>Character not found</div>;

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">{character.name}</h1>
      <div className="grid gap-4">
        <div>
          <strong>Background:</strong> {character.background}
        </div>
        <div>
          <strong>Ancestry:</strong> {character.ancestry}
        </div>
        <div>
          <strong>Class:</strong> {character.class}
        </div>
      </div>
    </div>
  );
} 