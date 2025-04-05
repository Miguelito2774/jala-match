import Login from '../app/(auth)/login/page';

export default function Home() {
  return (
    <div className="flex min-h-screen flex-col items-center bg-[#28292E]">
      <div className="flex w-full max-w-[1440px] flex-col">
        <main className="flex-grow">
          <Login />
        </main>
      </div>
    </div>
  );
}
