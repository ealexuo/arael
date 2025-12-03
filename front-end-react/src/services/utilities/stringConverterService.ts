
export const stringConverterService = {
  
  formatFieldId : (fileId: number) => {
      const valueString = fileId.toString();
      const year = valueString.substring(valueString.length - 4);
      const correlative = valueString.substring(0, valueString.length - 4);
      return `${correlative}-${year}`;
  }
  
};
