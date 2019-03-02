import * as React from 'react';
import * as styles from './listBox.scss';
import { IItemBoxProps, ItemBox } from '../itemBox/itemBox';

export interface IListBoxProps {
    id?: string;
    items?: IItemBoxProps[];
    noItemsMessage?: string;
    favouritesCanBeAdded?: boolean;
    favouritesCanBeRemoved?: boolean;
}

export class ListBox extends React.Component<IListBoxProps> {
    static defaultProps: Partial<IListBoxProps> = {
        id: "listBoxDefaultId",
        items: [],
        noItemsMessage: "No items.",
        favouritesCanBeAdded: false,
        favouritesCanBeRemoved: false
    };

    private getItems(): React.ReactNode {
        const items = this.props.items.map(item => (
            <ItemBox
                key={item.title}
                {...item}
                favouritesCanBeAdded={this.props.favouritesCanBeAdded}
                favouritesCanBeRemoved={this.props.favouritesCanBeRemoved} 
            />
        ));

        return items.length > 0 ? items : <li className={styles.noItems}>{this.props.noItemsMessage}</li>;
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
