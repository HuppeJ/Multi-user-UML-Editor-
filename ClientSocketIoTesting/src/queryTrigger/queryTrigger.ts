import * as $ from 'jquery';
import { IItemBoxProps } from '../itemBox/itemBox';

export interface ResponseResult {
    body: string;
    category: string;
    keywords: string;
    title: string;
}

export class QueryTrigger {
    public async getFilteredResults(inputText: string): Promise<IItemBoxProps[]> {
        const results = await this.getResults();
        const input: string = inputText.toUpperCase();

        const filteredResults = $.map(results, (result) => {
            const match = result.keywords.toUpperCase().indexOf(input) !== -1;
            return match ? this.parseResult(result) : null;
        });
        
        return filteredResults.reverse();
    }

    private async getResults(): Promise<ResponseResult[]> {
        let results: ResponseResult[] = [];
        await $.ajax({
            type: "GET",
            url: `https://secure.toronto.ca/cc_sr_v1/data/swm_waste_wizard_APR?limit=1000`,
            dataType: "json",
            cache: false,
            success: (data) => {
                results = data;
            },
            error: (e) => {
                results = [];
                alert(e);
            }
        });

        return results;
    }

    private parseResult(result: ResponseResult) {
        return {
            title: result.title,
            description: result.body
        } as IItemBoxProps;
    }
}
