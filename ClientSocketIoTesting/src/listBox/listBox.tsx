import * as React from 'react';
import * as styles from './listBox.scss';
import { IItemBoxProps, ItemBox } from '../itemBox/itemBox';

export interface IListBoxProps {
    id?: string;
    items?: IItemBoxProps[];
}

export class ListBox extends React.Component<IListBoxProps> {
    private getItems(): React.ReactNode {
        const items = this.props.items.map(item => (
            <ItemBox
                key={item.eventName}
                {...item}
            />
        ));

        return items.length > 0 ? items : <li className={styles.noItems}>No test tool</li>;
    }

    render() {
        return (
            <div className={styles.container}>
                <ul className={styles.list}>
                    {this.getItems()}
                </ul>
            </div>
        );
    }
}
