export function mapToObj(map: any){
    const obj:any = {};
    for (let [k,v] of map)
      obj[k] = v
    return obj
}