import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ClashOfClanService } from '../../../shared/services/clash-of-clan.service';
import { IPlayer } from '../../../shared/interfaces/player';
import { NgxLoadingModule } from 'ngx-loading';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-search-player',
  standalone: true,
  imports: [ReactiveFormsModule, NgxLoadingModule, RouterLink],
  templateUrl: './search-player.component.html',
  styles: ``
})
export class SearchPlayerComponent {
  playerForm: FormGroup = new FormGroup({
    playerTag: new FormControl("", [Validators.required, Validators.minLength(10), Validators.maxLength(11)]),
  });

  playerData?: IPlayer;

  isTroopsOpen: boolean = false;
  isAchievementOpen: boolean = false;
  isBuilderBaseOpen: boolean = false;
  isLoading : boolean = false;

  // allSuperTroops: [] = [];


  staticImg: { [key: string]: string } = {
    "Barbarian": "assets/Clasher/Avatar_Barbarian.webp",
    "Archer": "assets/Clasher/Avatar_Archer.webp",
    "Goblin": "assets/Clasher/Avatar_Goblin.webp",
    "Giant": "assets/Clasher/Avatar_Giant.webp",
    "Wall Breaker": "assets/Clasher/Avatar_Wall_Breaker.webp",
    "Balloon": "assets/Clasher/Avatar_Balloon.webp",
    "Wizard": "assets/Clasher/Avatar_Wizard.webp",
    "Healer": "assets/Clasher/Avatar_Healer.webp",
    "Dragon": "assets/Clasher/Avatar_Dragon.webp",
    "P.E.K.K.A": "assets/Clasher/Avatar_P.E.K.K.A.webp",
    "Baby Dragon": "assets/Clasher/Avatar_Baby_Dragon.webp",
    "Miner": "assets/Clasher/Avatar_Miner.webp",
    "Electro Dragon": "assets/Clasher/Avatar_Electro_Dragon.webp",
    "Yeti": "assets/Clasher/Avatar_Yeti.webp",
    "Dragon Rider": "assets/Clasher/Avatar_Dragon_Rider.webp",
    "Electro Titan": "assets/Clasher/Avatar_Electro_Titan.webp",
    "Root Rider": "assets/Clasher/Avatar_Root_Rider.webp",
    // Dark Troops
    "Minion": "assets/Clasher/Avatar_Minion.webp",
    "Valkyrie": "assets/Clasher/Avatar_Valkyrie.webp",
    "Hog Rider": "assets/Clasher/Avatar_Hog_Rider.webp",
    "Golem": "assets/Clasher/Avatar_Golem.webp",
    "Witch": "assets/Clasher/Avatar_Witch.webp",
    "Lava Hound": "assets/Clasher/Avatar_Lava_Hound.webp",
    "Bowler": "assets/Clasher/Avatar_Bowler.webp",
    "Ice Golem": "assets/Clasher/Avatar_Ice_Golem.webp",
    "Headhunter": "assets/Clasher/Avatar_Headhunter.webp",
    "Apprentice Warden": "assets/Clasher/Avatar_Apprentice_Warden.webp",
    // Super troops
    "Super Barbarian": "assets/Clasher/Avatar_Super_Barbarian.webp",
    "Super Archer": "assets/Clasher/Avatar_Super_Archer.webp",
    "Super Wall Breaker": "assets/Clasher/Avatar_Super_Wall_Breaker.webp",
    "Super Giant": "assets/Clasher/Avatar_Super_Giant.webp",
    "Sneaky Goblin": "assets/Clasher/Avatar_Sneaky_Goblin.webp",
    "Super Wizard": "assets/Clasher/Avatar_Super_Wizard.webp",
    "Inferno Dragon": "assets/Clasher/Avatar_Inferno_Dragon.webp",
    "Rocket Balloon": "assets/Clasher/Avatar_Rocket_Balloon.webp",
    "Super Miner": "assets/Clasher/Avatar_Super_Miner.webp",
    "Super Valkyrie": "assets/Clasher/Avatar_Super_Valkyrie.webp",
    "Super Witch": "assets/Clasher/Avatar_Super_Witch.webp",
    "Ice Hound": "assets/Clasher/Avatar_Ice_Hound.webp",
    "Super Dragon": "assets/Clasher/Avatar_Super_Dragon.webp",
    "Super Bowler": "assets/Clasher/Avatar_Super_Bowler.webp",
    "Super Minion": "assets/Clasher/Avatar_Super_Minion.webp",
    "Super Hog Rider": "assets/Clasher/Avatar_Super_Hog_Rider.webp",
    // Builder base troops
    "Raged Barbarian": "assets/Clasher/Raged_Barbarian_info.webp",
    "Sneaky Archer": "assets/Clasher/Sneaky_Archer_info.webp",
    "Beta Minion": "assets/Clasher/Beta_Minion_info.webp",
    "Boxer Giant": "assets/Clasher/Boxer_Giant_info.webp",
    "Bomber": "assets/Clasher/Bomber_info.webp",
    "Power P.E.K.K.A": "assets/Clasher/Super_P.E.K.K.A_info.webp",
    "Cannon Cart": "assets/Clasher/Cannon_Cart_info.webp",
    "Hog Glider": "assets/Clasher/Hog_Glider_info.webp",
    "Drop Ship": "assets/Clasher/Drop_Ship_info.webp",
    // "Baby Dragon": "assets/Clasher/PEKKA1", // used as same
    "Night Witch": "assets/Clasher/Night_Witch_info.webp",
    "Electrofire Wizard": "assets/Clasher/Electrofire_Wizard_info.webp",
    // Machines
    "Wall Wrecker": "assets/Clasher/Avatar_Wall_Wrecker.webp",
    "Battle Blimp": "assets/Clasher/Avatar_Battle_Blimp.webp",
    "Stone Slammer": "assets/Clasher/Avatar_Stone_Slammer.webp",
    "Siege Barracks": "assets/Clasher/Avatar_Siege_Barracks.webp",
    "Log Launcher": "assets/Clasher/Avatar_Log_Launcher.webp",
    "Flame Flinger": "assets/Clasher/Avatar_Flame_Flinger.webp",
    "Battle Drill": "assets/Clasher/Avatar_Battle_Drill.webp",
    // Pets
    "L.A.S.S.I": "assets/Clasher/Avatar_L.A.S.S.I.webp",
    "Mighty Yak": "assets/Clasher/Avatar_Mighty_Yak.webp",
    "Electro Owl": "assets/Clasher/Avatar_Electro_Owl.webp",
    "Unicorn": "assets/Clasher/Avatar_Unicorn.webp",
    "Phoenix": "assets/Clasher/Avatar_Phoenix.webp",
    "Poison Lizard": "assets/Clasher/Avatar_Poison_Lizard.webp",
    "Diggy": "assets/Clasher/Avatar_Diggy.webp",
    "Frosty": "assets/Clasher/Avatar_Frosty.webp",
    "Spirit Fox": "assets/Clasher/Avatar_Spirit_Fox.webp",
    // Heroes
    "Barbarian King": "assets/Clasher/Barbarian_King_info.webp",
    "Archer Queen": "assets/Clasher/Archer_Queen_info.webp",
    "Royal Champion": "assets/Clasher/Royal_Champion_info.webp",
    "Grand Warden": "assets/Clasher/Grand_Warden.webp",
    "Battle Machine": "assets/Clasher/Battle_Machine_info.webp",
    "Battle Copter": "assets/Clasher/Battle_Copter_info.webp",
    // Hero equipments
    "Giant Gauntlet": "assets/Clasher/Giant_Gauntlet.webp",
    "Barbarian Puppet": "assets/Clasher/Barbarian_Puppet.webp",
    "Rage Vial": "assets/Clasher/Rage_Vial.webp",
    "Archer Puppet": "assets/Clasher/Archer_Puppet.webp",
    "Invisibility Vial": "assets/Clasher/Invisibility_Vial.webp",
    "Eternal Tome": "assets/Clasher/Eternal_Tome.webp",
    "Life Gem": "assets/Clasher/Life_Gem.webp",
    "Seeking Shield": "assets/Clasher/Seeking_Shield.webp",
    "Royal Gem": "assets/Clasher/Royal_Gem.webp",
    "Healer Puppet": "assets/Clasher/Healer_Puppet.webp",
    "Rage Gem": "assets/Clasher/Rage_Gem.webp",
    "Earthquake Boots": "assets/Clasher/Earthquake_Boots.webp",
    "Vampstache": "assets/Clasher/Vampstache.webp",
    "Giant Arrow": "assets/Clasher/Giant_Arrow.webp",
    "Healing Tome": "assets/Clasher/Healing_Tome.webp",
    "Frozen Arrow": "assets/Clasher/Frozen_Arrow.webp",
    "Hog Rider Puppet": "assets/Clasher/Hog_Rider_Puppet.webp",
    "Haste Vial": "assets/Clasher/Haste_Vial.webp",
    // Spells
    "Lightning Spell": "assets/Clasher/Lightning_Spell_info.jpg",
    "Healing Spell": "assets/Clasher/Healing_Spell_info.webp",
    "Rage Spell": "assets/Clasher/Rage_Spell_info.webp",
    "Jump Spell": "assets/Clasher/Jump_Spell_info.webp",
    "Freeze Spell": "assets/Clasher/Freeze_Spell_info.webp",
    "Invisibility Spell": "assets/Clasher/Invisibility_Spell_info.webp",
    "Recall Spell": "assets/Clasher/Recall_Spell_info.webp",
    "Clone Spell": "assets/Clasher/Clone_Spell_info.webp",
    "Poison Spell": "assets/Clasher/Poison_Spell_info.webp",
    "Earthquake Spell": "assets/Clasher/Earthquake_Spell_info.webp",
    "Haste Spell": "assets/Clasher/Haste_Spell_info.webp",
    "Skeleton Spell": "assets/Clasher/Skeleton_Spell_info.webp",
    "Bat Spell": "assets/Clasher/Bat_Spell_info.webp",
    "Overgrowth Spell": "assets/Clasher/Overgrowth_Spell_info.webp",
  };

  constructor(private clashingService: ClashOfClanService, private toaster: ToastrService) {
    const data = localStorage.getItem("player");
    this.playerData = JSON.parse(data!);
  }


  onSearchPlayer() {
    if (this.playerForm.valid) {
      this.isLoading = true;
      // check local storage player tag if there
      const data = localStorage.getItem("player");
      // console.log("plyerrrr", this.playerData);
      
      if (data === null || data === undefined || JSON.parse(data!).tag !== this.playerForm.value.playerTag) {
        
        this.clashingService.getPlayer(this.playerForm.value.playerTag).subscribe({
          next: res => {
            if (res.result !== null) {
              this.isLoading = false;
              // console.log("Player response", res);
              this.playerData = res.result;
              // storing in local storage for dev purpose to restrict api call
              localStorage.setItem("player", JSON.stringify(res.result));
              this.toaster.success("Player found", "Player  found!!");
            }
            else {
              this.isLoading = false;
              this.toaster.error(res.message, "No player  found!!");
            }
          },
          error: err => {
            this.isLoading = false;
            // console.log(err)
          }
        });
        // window.location.reload();
      }
      else {
        // get from localstorage
        this.isLoading = false;
        this.playerData = JSON.parse(data!);
        this.toaster.success("Player found", "Player  found!!");
      }
    }
    else {
      this.toaster.error("Put player tag ...", "Search Player");
    }
  }

  onClear() {
    this.playerForm.reset();
  }

  onClickTroops() {
    this.isTroopsOpen = true;
    this.isAchievementOpen = false; // make other false
    this.isBuilderBaseOpen = false;
  }
  onClickBuilderBase() {
    this.isTroopsOpen = false;
    this.isBuilderBaseOpen = true;
    this.isAchievementOpen = false;
  }
  onClickAchievement() {
    this.isTroopsOpen = false;
    this.isAchievementOpen = true;
    this.isBuilderBaseOpen = false;
  }
}
