import * as React from 'react';
import * as ReactDOM from 'react-dom';
import * as styles from './app.scss';
import * as Redux from 'redux';
import { RootReducers } from './reducers';
import { Provider } from 'react-redux';
import { WasteLookupAppConnected } from '../wasteLookupApp/wasteLookupAppConnected';
import { IAppState } from './appState';

export const AppStore: Redux.Store<IAppState> = Redux.createStore(RootReducers);

class App extends React.Component {
    render() {
        return (
            <Provider store={AppStore}>
                <div className={styles.flexContainer}>
                    <div className={styles.flexItemCenter}>
                        <WasteLookupAppConnected />
                    </div>
                </div>
            </Provider>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('app'));
