import * as React from 'react';
import { SearchBar } from '../searchBar/searchBar';
import { ListBox } from '../listBox/listBox';
import { IItemBoxProps } from '../itemBox/itemBox';
import { QueryTrigger } from '../queryTrigger/queryTrigger'

export interface ISearchEngineProps { }

export interface ISearchEngineState {
    results: IItemBoxProps[];
    isSearching: boolean;
}

export class SearchEngine extends React.Component<ISearchEngineProps, ISearchEngineState> {
    public static readonly ID = "SEARCH_ENGINE_ID";
    private queryTrigger: QueryTrigger;

    constructor(props: ISearchEngineProps) {
        super(props);
        this.state = {
            results: [],
            isSearching: false
        }

        this.queryTrigger = new QueryTrigger();
    }

    private async onSearch(inputText: string): Promise<void> {
        this.setState({ isSearching: true });

        const newResults = await this.queryTrigger.getFilteredResults(inputText);
        this.setState({ results: newResults });

        this.setState({ isSearching: false });
    }

    private onEmptyInput(): void {
        this.setState({ results: [] });
    }

    render() {
        return (
            <div>
                <SearchBar
                    onSearch={(inputText) => this.onSearch(inputText)}
                    onEmptyInput={() => this.onEmptyInput()}
                    placeholder={"Search the waste wizard.."}
                    isSearching={this.state.isSearching}
                />
                <ListBox
                    id={SearchEngine.ID}
                    items={this.state.results}
                    noItemsMessage={"No matching results."}
                    favouritesCanBeAdded
                />
            </div>
        );
    }

}
