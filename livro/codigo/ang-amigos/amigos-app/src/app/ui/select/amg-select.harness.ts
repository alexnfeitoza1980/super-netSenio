import { ComponentHarness } from '@angular/cdk/testing';

export class AmgSelectHarness extends ComponentHarness {
  static hostSelector = 'amg-select';

  private locateTrigger = this.locatorFor('.amg-select-trigger');
  private locateDropdown = this.locatorForOptional('.amg-select-dropdown');
  private locateItems = this.locatorForAll('.amg-select-item');

  async isOpen(): Promise<boolean> {
    const dropdown = await this.locateDropdown();
    return dropdown !== null;
  }

  async getSelectedText(): Promise<string> {
    const trigger = await this.locateTrigger();
    return trigger.text();
  }

  async open(): Promise<void> {
    if (!(await this.isOpen())) {
      const trigger = await this.locateTrigger();
      await trigger.click();
    }
  }

  async getOptionsCount(): Promise<number> {
    await this.open();
    const items = await this.locateItems();
    return items.length;
  }

  async selectOptionByIndex(index: number): Promise<void> {
    await this.open();
    const items = await this.locateItems();
    if (items[index]) {
      await items[index].click();
    }
  }
}

