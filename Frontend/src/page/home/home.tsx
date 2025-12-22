import { useEffect, useState } from 'react';
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
            
            <br />
            Response from backend: { data }
        </DummyComponent>
    )
}
