import { useEffect, useState } from 'react';
import { Link } from 'react-router';
import { DummyComponent } from '../../component/dummy/DummyComponent.tsx';
import { useAPI } from '../../context/services.tsx';

export function Home() {
    const api = useAPI()

    const [data, setData] = useState<string>('')

    useEffect(() => {
        api.getHelloUniversity().then(result => setData(result.value))
    }, []);

    return (
        <DummyComponent prop={ 'home' }>
            <Link to={ 'page1' }>
                Go to Page 1
            </Link>
            <br />
            Response from backend: { data }
        </DummyComponent>
    )
}
