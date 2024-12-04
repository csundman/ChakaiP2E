"use client";

import { useEffect, useState } from 'react';
import { Grid } from "../components/Grid";
import { useRouter } from 'next/navigation';

interface Character {
  id: number;
  name: string;
  background: string;
  ancestry: string;
  class: string;
}

// Add dummy data constant - let's verify it's defined
const DUMMY_CHARACTERS: Character[] = [
  { id: 1, name: 'Example Character', background: 'Acolyte', ancestry: 'Human', class: 'Fighter' },
  { id: 2, name: 'Sample Hero', background: 'Criminal', ancestry: 'Elf', class: 'Wizard' },
];

export function CharacterListComponent() {
  const router = useRouter();
  const [characters, setCharacters] = useState<Character[]>(DUMMY_CHARACTERS);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchCharacters() {
      try {
        console.log('Attempting to fetch characters...');
        const res = await fetch('your-api-endpoint/characters');
        if (!res.ok) {
          throw new Error('Failed to fetch characters');
        }
        const data = await res.json();
        setCharacters(data);
      } catch (err) {
        console.log('Error caught, keeping dummy data:', DUMMY_CHARACTERS);
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setIsLoading(false);
      }
    }

    fetchCharacters();
  }, []);

  // Add debug log to see current characters state
  console.log('Current characters:', characters);

  const handleRowClick = (character: Character) => {
    router.push(`/characters/${character.id}`);
  };

  if (isLoading) return <div>Loading characters...</div>;

  return (
    <div>
      {error && <div>Error: {error}</div>}
      <h1>Characters</h1>
      <Grid
        data={characters}
        columns={[
          { field: 'id', header: 'ID' },
          { field: 'name', header: 'Name' },
          { field: 'background', header: 'Background' },
          { field: 'ancestry', header: 'Ancestry' },
          { field: 'class', header: 'Class' },
        ]}
        onRowClick={handleRowClick}
      />
    </div>
  );
}