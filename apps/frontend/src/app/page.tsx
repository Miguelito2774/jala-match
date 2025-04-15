// app/page.tsx
import { redirect } from 'next/navigation';

import { ROUTES } from '../../routes';

export default function Home() {
  redirect(ROUTES.MANAGER.TEAM_BUILDER);
}
