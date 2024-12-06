export default function HomeComponent() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[60vh] space-y-8">
      <svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 128 96"
        className="w-64 h-48"
        fill="none"
        stroke="currentColor"
        strokeWidth="2"
      >

        {/* Cup (Center) */}
        <g transform="translate(25, 26)">
          {/* Cup */}
          <path
            d="M16 40c0 8.837 7.163 16 16 16s16-7.163 16-16H16z"
            fill="#c0c0c0"
          />
          {/* Handle */}
          <path
            d="M48 32v0c4.418 0 8 3.582 8 8s-3.582 8-8 8"
            fill="none"
            stroke="#c0c0c0"
          />
          {/* Tea */}
          <path
            d="M16 32h32v8H16z"
            fill="#d2b48c"
          />
          {/* Steam */}
          <path
            d="M24 16c0 4 4 4 4 8s-4 4-4 8"
            fill="none"
            stroke="#c0c0c0"
          />
          <path
            d="M36 16c0 4 4 4 4 8s-4 4-4 8"
            fill="none"
            stroke="#c0c0c0"
          />
        </g>
      </svg>
      <h1 className="text-2xl font-bold">Welcome to Chakai P2E!</h1>
    </div>
  );
}
