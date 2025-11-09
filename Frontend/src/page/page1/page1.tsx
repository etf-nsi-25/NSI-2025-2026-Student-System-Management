import { Link } from 'react-router';
import { DummyComponent } from '../../component/dummy/DummyComponent.tsx';


export function Page1() {
    return (
        <DummyComponent prop={'page1'}>
            <Link to={'/'}>
                Go to Home
            </Link>
        </DummyComponent>
    )
}
