import type { PropsWithChildren } from 'react';
import { useState } from 'react';
import { useDummyContext } from '../../context/dummy/dummy-context.tsx';

export interface DummyComponentProps {
    prop: string
}

export function DummyComponent({ prop, children }: PropsWithChildren<DummyComponentProps>) {
    const { data, setData } = useDummyContext()
    const [text, setText] = useState('');

    return (
        <div>
            Dummy Component. <br />

            Props Data: { prop } <br />


            Context Data: { data } <br />
            Input: <input value={ text } onChange={ e => setText(e.target.value) } /> <br />
            <button onClick={ () => setData(text) }>Update Context:</button> <br />

            Children: <br />
            { children }
        </div>
    )
}
