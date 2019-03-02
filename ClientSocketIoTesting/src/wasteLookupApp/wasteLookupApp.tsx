import * as React from 'react';
import * as styles from './wasteLookupApp.scss';
import { IItemBoxProps } from '../itemBox/itemBox';
import { TestTools } from '../testTools/TestTools';

export interface IWasteLookupAppStateProps {
    favouritedItems?: IItemBoxProps[];
}

export interface IWasteLookupAppProps extends IWasteLookupAppStateProps {}

export class WasteLookupApp extends React.Component<IWasteLookupAppProps> {
    render() {
        return (
            <div>
                <div className={styles.container}>
                    <div className={styles.titleContainer}>
                        <div className={styles.titleContent}>
                            PolyPaint Server Testing Tool
                        </div>
                    </div>
                </div>
                <TestTools />
            </div>
        );
    }
}
