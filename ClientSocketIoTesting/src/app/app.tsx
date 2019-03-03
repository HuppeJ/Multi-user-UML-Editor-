import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as styles from './app.scss';
import { TestingTool } from '../testingTool/TestingTool';


class App extends React.Component {

    constructor(props: any) {
        super(props);



    }

    render() {
        return (
            <div className={styles.flexContainer}>
                <div className={styles.flexItemCenter}>
                    <TestingTool />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('app'));
