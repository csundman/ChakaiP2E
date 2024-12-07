import { DUMMY_CHARACTERS } from '../data';
import CharacterDetails from './CharacterDetails';

export async function generateStaticParams() {
  return DUMMY_CHARACTERS.map((character) => ({
    id: character.id.toString(),
  }));
}

export default function Page() {
  return <CharacterDetails />;
} 