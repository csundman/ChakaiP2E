"use client";

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useCharacters } from '../contexts/CharacterContext';

export function Breadcrumb() {
  const pathname = usePathname();
  const { characters } = useCharacters();
  
  const segments = pathname?.split('/')
    .filter(segment => segment)
    .map((segment, index, array) => {
      const path = '/' + array.slice(0, index + 1).join('/');
      
      // Check if this segment is a character ID
      if (segment.match(/^\d+$/) && array[index - 1] === 'characters') {
        const characterId = parseInt(segment);
        const character = characters.find(c => c.id === characterId);
        return { 
          label: character ? character.name : segment,
          path 
        };
      }
      
      const label = segment
        .split('-')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');

      return { label, path };
    });

  return (
    <nav className="flex py-4 text-sm">
      <ol className="flex items-center space-x-2">
        <li>
          <Link 
            href="/" 
            className="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"
          >
            Home
          </Link>
        </li>
        {segments.map((segment, index) => (
          <li key={segment.path} className="flex items-center">
            <span className="mx-2 text-gray-400 dark:text-gray-600">/</span>
            {index === segments.length - 1 ? (
              <span className="text-gray-900 dark:text-gray-100">
                {segment.label}
              </span>
            ) : (
              <Link 
                href={segment.path}
                className="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"
              >
                {segment.label}
              </Link>
            )}
          </li>
        ))}
      </ol>
    </nav>
  );
} 