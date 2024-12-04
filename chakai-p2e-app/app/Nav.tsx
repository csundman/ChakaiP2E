import Link from 'next/link';
export default function Nav({ className = '' }: { className?: string }) {
  return (
    <nav className={`bg-gray-800 text-white p-4 w-full h-full min-h-screen sm:min-h-0 ${className}`}>
      <ul className="flex flex-col gap-4">
        <li>
          <Link href="/" className="hover:text-gray-300">
            Home
          </Link>
        </li>
        <li>
          <Link href="/characters" className="hover:text-gray-300">
            Characters
          </Link>
        </li>
      </ul>
    </nav>
  );
}
